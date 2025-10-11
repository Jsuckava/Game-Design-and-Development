import os
import random
import time
import tkinter as tk
from tkinter import messagebox

from PIL import Image, ImageTk

# SOUND: pygame mixer for non-blocking short SFX
try:
    import pygame

    pygame.mixer.init()
    PYGAME_AVAILABLE = True
except Exception:
    PYGAME_AVAILABLE = False

# ----------------- Config -----------------
rows, cols = 4, 4
card_size = 100
reveal_delay = 700
game_time = 30

flip_steps = 8
flip_delay = 10

base_dir = os.path.dirname(__file__)
asset_dir = os.path.join(base_dir, "assets")

# Face image assets (at least (rows*cols)//2 of these)
assets = [
    os.path.join(asset_dir, "7.png"),
    os.path.join(asset_dir, "8.png"),
    os.path.join(asset_dir, "9.png"),
    os.path.join(asset_dir, "10.png"),
    os.path.join(asset_dir, "11.png"),
    os.path.join(asset_dir, "12.png"),
    os.path.join(asset_dir, "13.png"),
    os.path.join(asset_dir, "14.png"),
    os.path.join(asset_dir, "15.png"),
    os.path.join(asset_dir, "16.png"),
    os.path.join(asset_dir, "17.png"),
    os.path.join(asset_dir, "18.png"),
]
card_back = os.path.join(asset_dir, "back.png")

# Sound filenames expected in assets/
SND_FLIP = os.path.join(asset_dir, "flip.mp3")
SND_MATCH = os.path.join(asset_dir, "match.mp3")
SND_MISMATCH = os.path.join(asset_dir, "mismatch.mp3")
SND_WIN = os.path.join(asset_dir, "win.mp3")
SND_LOSE = os.path.join(asset_dir, "lose.mp3")
SND_TICK = os.path.join(asset_dir, "tick.wav")  # optional per-second tick

pairs_needed = (rows * cols) // 2
if len(assets) < pairs_needed:
    raise SystemExit(f"Need at least {pairs_needed} unique face images in assets.")


class MemoryGame:
    def __init__(self, root):
        self.root = root
        self.root.title("Memory Matching Game")
        self.scheduled_after_ids = []
        self._flip_frame_cache = {}

        # sound control
        self.sound_enabled = True  # toggle this to mute/unmute
        self.sounds = {}
        self._init_sounds()

        # Load face images (keep both PIL + Tk versions)
        self.face_pils = {}
        self.face_images = {}
        for idx, path in enumerate(assets[:pairs_needed]):
            if not os.path.exists(path):
                raise SystemExit(f"Missing asset: {path}")
            pil = (
                Image.open(path)
                .convert("RGBA")
                .resize((card_size, card_size), Image.Resampling.LANCZOS)
            )
            self.face_pils[idx] = pil
            self.face_images[idx] = ImageTk.PhotoImage(pil)

        # Load back image
        if not os.path.exists(card_back):
            raise SystemExit(f"Missing back image: {card_back}")
        self.back_pil = (
            Image.open(card_back)
            .convert("RGBA")
            .resize((card_size, card_size), Image.Resampling.LANCZOS)
        )
        self.back_image = ImageTk.PhotoImage(self.back_pil)

        self.deck = []
        self.card_states = []
        self.flipped_indices = []
        self.locked = True
        self.matched_pairs = 0
        self.moves = 0

        self.time_left = game_time
        self.timer_running = False
        self.timer_after_id = None

        # Topbar UI
        self.topbar = tk.Frame(root)
        self.topbar.pack(fill="x", pady=6)
        self.moves_label = tk.Label(
            self.topbar, text=f"moves: {self.moves}".lower(), font=("Segoe UI", 11)
        )
        self.moves_label.pack(side="left", padx=8)
        self.matches_label = tk.Label(
            self.topbar,
            text=f"matches: {self.matched_pairs}/{pairs_needed}".lower(),
            font=("Segoe UI", 11),
        )
        self.matches_label.pack(side="left", padx=8)
        self.timer_label = tk.Label(
            self.topbar,
            text=f"time: {self._format_time(self.time_left)}".lower(),
            font=("Segoe UI", 11),
        )
        self.timer_label.pack(side="left", padx=8)

        # Mute toggle button
        self.mute_btn = tk.Button(self.topbar, text="mute", command=self.toggle_sound)
        self.mute_btn.pack(side="right", padx=8)

        self.restart_btn = tk.Button(self.topbar, text="restart", command=self.restart)
        self.restart_btn.pack(side="right", padx=8)

        # Board UI
        self.board_frame = tk.Frame(
            root,
            width=(card_size + 12) * cols,
            height=(card_size + 12) * rows,
            bg="#222",
        )
        self.board_frame.pack(padx=10, pady=6)
        self.board_frame.update_idletasks()

        self._create_new_deck()
        self._build_board()
        self.root.after(800, self._show_then_hide)

    # ---------------- Sound helpers ----------------
    def _init_sounds(self):
        """Try to initialize pygame.mixer and load all SFX if available."""
        if not PYGAME_AVAILABLE:
            # mixer not available; skip sounds
            return
        def try_load(path):
            if os.path.exists(path):
                try:
                    snd = pygame.mixer.Sound(path)
                    return snd
                except Exception:
                    return None
            return None

        self.sounds["flip"] = try_load(SND_FLIP)
        self.sounds["match"] = try_load(SND_MATCH)
        self.sounds["mismatch"] = try_load(SND_MISMATCH)
        self.sounds["win"] = try_load(SND_WIN)
        self.sounds["lose"] = try_load(SND_LOSE)
        self.sounds["tick"] = try_load(SND_TICK)

        # set sensible default volumes (0.0 - 1.0)
        for k, s in self.sounds.items():
            if s:
                s.set_volume(0.8)

    def play_sound(self, key):
        """Play a sound by key (non-blocking). Graceful if pygame missing or file absent."""
        if not self.sound_enabled:
            return
        if not PYGAME_AVAILABLE:
            return
        snd = self.sounds.get(key)
        if snd:
            try:
                snd.play()
            except Exception:
                pass

    def toggle_sound(self):
        self.sound_enabled = not self.sound_enabled
        self.mute_btn.config(text="unmute" if not self.sound_enabled else "mute")
        # optional: stop currently playing sounds when muted
        if not self.sound_enabled and PYGAME_AVAILABLE:
            try:
                pygame.mixer.stop()
            except Exception:
                pass

    # ----------------- Utilities -----------------
    def _format_time(self, sec):
        return time.strftime("%M:%S", time.gmtime(max(0, int(sec))))

    def generate_flip_frames(self, pil_img, steps=flip_steps):
        key = (id(pil_img), steps)
        if key in self._flip_frame_cache:
            return self._flip_frame_cache[key]
        frames = []
        w, h = pil_img.size
        for i in range(steps, 0, -1):
            frac = i / steps
            new_w = max(1, int(round(w * frac)))
            resized = pil_img.resize((new_w, h), Image.Resampling.LANCZOS)
            canvas = Image.new("RGBA", (w, h), (0, 0, 0, 0))
            offset_x = (w - new_w) // 2
            canvas.paste(resized, (offset_x, 0), resized)
            frames.append(ImageTk.PhotoImage(canvas))
        self._flip_frame_cache[key] = frames
        return frames

    def animate_flip(self, idx, to_revealed=True, on_complete=None):
        # play flip sound at start
        self.play_sound("flip")

        self.locked = True
        btn = self.buttons[idx]
        face_id = self.deck[idx]

        front_pil = self.face_pils[face_id]
        back_pil = self.back_pil

        front_shrink = self.generate_flip_frames(front_pil)
        back_shrink = self.generate_flip_frames(back_pil)

        if to_revealed:
            frames = back_shrink + list(reversed(front_shrink))
        else:
            frames = front_shrink + list(reversed(back_shrink))

        def schedule_frame(i):
            if i >= len(frames):
                self.card_states[idx] = "revealed" if to_revealed else "hidden"
                self.locked = False
                if callable(on_complete):
                    on_complete()
                return
            frame_img = frames[i]
            btn.config(image=frame_img)
            btn.image_ref = frame_img
            self.root.after(flip_delay, lambda: schedule_frame(i + 1))

        schedule_frame(0)

    # ----------------- Setup / Board -----------------
    def _create_new_deck(self):
        pool = list(range(pairs_needed))
        deck = pool + pool
        random.shuffle(deck)
        self.deck = deck
        self.card_states = ["revealed"] * (rows * cols)
        self.flipped_indices = []
        self.locked = True
        self.matched_pairs = 0
        self.moves = 0
        self.time_left = game_time
        self.timer_running = False
        self._clear_scheduled()

    def _clear_scheduled(self):
        for a_id in self.scheduled_after_ids:
            try:
                self.root.after_cancel(a_id)
            except Exception:
                pass
        self.scheduled_after_ids.clear()
        if self.timer_after_id:
            try:
                self.root.after_cancel(self.timer_after_id)
            except Exception:
                pass
            self.timer_after_id = None

    def _build_board(self):
        for child in self.board_frame.winfo_children():
            child.destroy()
        self.buttons = []

        padding = 10
        offset_x = 10
        offset_y = 10
        for idx, face_id in enumerate(self.deck):
            r, c = divmod(idx, cols)
            x = offset_x + c * (card_size + padding)
            y = offset_y + r * (card_size + padding)
            btn = tk.Button(
                self.board_frame,
                image=self.face_images[face_id],
                bd=2,
                relief="raised",
                command=lambda i=idx: self.on_card_click(i),
            )
            btn.place(x=x, y=y)
            btn.face_id = face_id
            self.buttons.append(btn)
            self.card_states[idx] = "revealed"

        self.moves_label.config(text=f"moves: {self.moves}".lower())
        self.matches_label.config(text=f"matches: {self.matched_pairs}/{pairs_needed}".lower())
        self.timer_label.config(text=f"time: {self._format_time(self.time_left)}".lower())

    def _show_then_hide(self):
        def hide_all():
            for idx, btn in enumerate(self.buttons):
                # animate flip to back (with sound)
                self.animate_flip(idx, to_revealed=False)
            self.flipped_indices = []
            self.locked = False
            self.moves = 0
            self.matched_pairs = 0
            self.moves_label.config(text=f"moves: {self.moves}".lower())
            self.matches_label.config(
                text=f"matches: {self.matched_pairs}/{pairs_needed}".lower()
            )
            self.timer_label.config(text=f"time: {self._format_time(self.time_left)}".lower())

        af = self.root.after(1500, hide_all)
        self.scheduled_after_ids.append(af)

    # ----------------- Core gameplay -----------------
    def on_card_click(self, idx):
        if self.locked:
            return
        if self.card_states[idx] != "hidden":
            return
        if not self.timer_running:
            self.start_timer()

        # optimistic mark
        self.card_states[idx] = "revealed"
        self.flipped_indices.append(idx)

        def after_reveal():
            # if two cards revealed, schedule check
            if len(self.flipped_indices) == 2:
                self.locked = True
                af = self.root.after(reveal_delay, self._check_pending_pair)
                self.scheduled_after_ids.append(af)
            else:
                self.locked = False

        # animate flip to shown (plays flip sound in animate_flip)
        self.animate_flip(idx, to_revealed=True, on_complete=after_reveal)

    def _check_pending_pair(self):
        if len(self.flipped_indices) < 2:
            self.flipped_indices.clear()
            self.locked = False
            return

        a, b = self.flipped_indices[0], self.flipped_indices[1]
        if self.card_states[a] != "revealed" or self.card_states[b] != "revealed":
            self.flipped_indices.clear()
            self.locked = False
            return

        self.moves += 1
        self.moves_label.config(text=f"moves: {self.moves}".lower())

        if self.deck[a] == self.deck[b]:
            # match sound + keep revealed
            self.play_sound("match")
            self.card_states[a] = "matched"
            self.card_states[b] = "matched"
            self.buttons[a].config(relief="sunken", state="disabled")
            self.buttons[b].config(relief="sunken", state="disabled")
            self.matched_pairs += 1
            self.matches_label.config(
                text=f"matches: {self.matched_pairs}/{pairs_needed}".lower()
            )
            self.flipped_indices.clear()
            self.locked = False
            if self.matched_pairs == pairs_needed:
                # small delay before win sound + dialog
                self.root.after(200, self._win)
        else:
            # mismatch sound and animate both back to hide
            self.play_sound("mismatch")
            remaining = {"count": 2}

            def make_cb():
                def cb():
                    remaining["count"] -= 1
                    if remaining["count"] == 0:
                        self.flipped_indices.clear()
                        self.locked = False

                return cb

            self.animate_flip(a, to_revealed=False, on_complete=make_cb())
            self.animate_flip(b, to_revealed=False, on_complete=make_cb())

    # ----------------- Timer & End -----------------
    def start_timer(self):
        if self.timer_running:
            return
        self.timer_running = True
        # optional: play tick sound each second
        self._tick_timer()

    def _tick_timer(self):
        # update label
        self.timer_label.config(text=f"time: {self._format_time(self.time_left)}".lower())
        # optional tick sound (comment out if annoying)
        # self.play_sound("tick")

        if self.time_left <= 0:
            self.timer_running = False
            self._game_over()
            return
        self.time_left -= 1
        self.timer_after_id = self.root.after(1000, self._tick_timer)
        self.scheduled_after_ids.append(self.timer_after_id)

    def _game_over(self):
        # play lose sound
        self.play_sound("lose")
        self.locked = True
        for idx, btn in enumerate(self.buttons):
            if self.card_states[idx] != "matched":
                btn.config(state="disabled")
        if messagebox.askyesno("game over", "time's up — game over. restart?"):
            self.restart()

    def _win(self):
        # play win sound
        self.play_sound("win")
        self.locked = True
        if self.timer_after_id:
            try:
                self.root.after_cancel(self.timer_after_id)
            except Exception:
                pass
            self.timer_after_id = None
        elapsed = game_time - self.time_left
        if messagebox.askyesno(
            "you win!",
            f"you matched all pairs!\nmoves: {self.moves}\ntime used: {self._format_time(elapsed)}\n\nplay again?",
        ):
            self.restart()

    # ----------------- Restart -----------------
    def restart(self):
        # stop any scheduled callbacks and sounds then rebuild
        self._clear_scheduled()
        if PYGAME_AVAILABLE:
            try:
                pygame.mixer.stop()
            except Exception:
                pass
        self._create_new_deck()
        self._build_board()
        self.root.after(800, self._show_then_hide)


if __name__ == "__main__":
    root = tk.Tk()
    app = MemoryGame(root)
    root.mainloop()
