import pygame
import sys
import random
import speech_recognition as sr
import threading

pygame.init()

WIDTH, HEIGHT = 1000, 700
CONFIG = {
    "WIDTH": WIDTH,
    "HEIGHT": HEIGHT,
    "TITLE": "Guess the Word (Type or Voice)",
    "COLORS": {
        "bg": "White",
        "text": "Black",    
        "title": "Black",
        "correct": "Green",
        "wrong": "Red",
        "input_box": "LightGray",
        "button": "Gray",
        "button_hover": "DarkGray"
    },
    "FONTS": {
        "main": ("Roboto", 28),
        "big": ("Arial", 50)
    },
    "LAYOUT": {
        "title_y": 60,
        "riddle_y": 180,
        "input_box": (50, 300, 900, 60),
        "score_pos": (850, 40),
        "riddle_width": 900,
        "button_rect": (WIDTH//2 - 200, 500, 400, 80)
    }
}

win = pygame.display.set_mode((WIDTH, HEIGHT))
pygame.display.set_caption(CONFIG["TITLE"])

font = pygame.font.SysFont(*CONFIG["FONTS"]["main"])
big_font = pygame.font.SysFont(*CONFIG["FONTS"]["big"])

GTW = [
    {"q": "Hindi tao, hindi hayop, pero mayroon itong loob.", "a": "Itlog"},
    {"q": "May paa walang kamay, may mukha walang bunganga.", "a": "Sapatos"},
    {"q": "Kung kailan mo pinatay, saka humaba ang buhay.", "a": "Kandila"},
    {"q": "Lumakad nang lumakad, hindi naman makalakad.", "a": "Orasan"},
    {"q": "May puno walang bunga, may dahon walang sanga.", "a": "Libro"},
    {"q": "Dalawang batong itim, malayo ang nararating.", "a": "Mata"},
    {"q": "Buto’t balat, lumilipad.", "a": "Saranggola"},
    {"q": "May dila, walang bibig.", "a": "Sapatos"},
    {"q": "Araw-araw naghihintay, pero hindi ka naman inaanyayahan.", "a": "Upuan"},
    {"q": "Mataas kapag bata, mababa kapag matanda.", "a": "Kandila"}
]

random.shuffle(GTW)
recognizer = sr.Recognizer()

current = 0
score = 0
game_over = False
message = ""
user_text = ""
listening = False

def parse_color(c):
    return pygame.Color(c) if isinstance(c, str) else c

def draw_text(text, font, color, x, y, center=False):
    render = font.render(text, True, color)
    rect = render.get_rect(center=(x, y) if center else (x, y))
    win.blit(render, rect)

def draw_wrapped_text_centered(text, font, color, center_x, y, max_width):
    words = text.split(" ")
    lines = []
    line = ""
    for word in words:
        test_line = line + word + " "
        if font.size(test_line)[0] <= max_width:
            line = test_line
        else:
            lines.append(line)
            line = word + " "
    lines.append(line)
    for i, l in enumerate(lines):
        render = font.render(l, True, color)
        rect = render.get_rect(center=(center_x, y + i * font.get_height()))
        win.blit(render, rect)

def check_answer(answer):
    global current, score, game_over, message, user_text
    if answer.strip().lower() == GTW[current]["a"].lower():
        score += 1
        current += 1
        user_text = ""
        if current >= len(GTW):
            game_over = True
            message = "Tapos na ang laro!"
    else:
        game_over = True
        message = f"Mali ang sagot! Ang sagot mo ay: {answer}"

def listen_and_check():
    global message, listening
    listening = True
    with sr.Microphone() as source:
        try:
            recognizer.adjust_for_ambient_noise(source, duration=1)
            audio = recognizer.listen(source, timeout=5, phrase_time_limit=5)
            answer = recognizer.recognize_google(audio, language="fil-PH")
            check_answer(answer)
        except Exception as e:
            message = f"Error: {e}"
    listening = False

def start_listening():
    thread = threading.Thread(target=listen_and_check, daemon=True)
    thread.start()

def draw_game(mouse_pos):
    win.fill(parse_color(CONFIG["COLORS"]["bg"]))
    if not game_over:
        draw_text(CONFIG["TITLE"], big_font, parse_color(CONFIG["COLORS"]["title"]), WIDTH//2, CONFIG["LAYOUT"]["title_y"], True)
        draw_wrapped_text_centered(GTW[current]["q"], font, parse_color(CONFIG["COLORS"]["text"]),
                                   WIDTH // 2, CONFIG["LAYOUT"]["riddle_y"], CONFIG["LAYOUT"]["riddle_width"])
        draw_text(f"Score: {score}", font, parse_color(CONFIG["COLORS"]["text"]), *CONFIG["LAYOUT"]["score_pos"])
        
        input_rect = pygame.Rect(*CONFIG["LAYOUT"]["input_box"])
        pygame.draw.rect(win, parse_color(CONFIG["COLORS"]["input_box"]), input_rect, border_radius=8)
        input_render = font.render(user_text, True, parse_color(CONFIG["COLORS"]["text"]))
        win.blit(input_render, (input_rect.x + 10, input_rect.y + (input_rect.height - input_render.get_height())//2))
        
        button_rect = pygame.Rect(*CONFIG["LAYOUT"]["button_rect"])
        color = parse_color(CONFIG["COLORS"]["button_hover"]) if button_rect.collidepoint(mouse_pos) else parse_color(CONFIG["COLORS"]["button"])
        pygame.draw.rect(win, color, button_rect, border_radius=8)
        draw_text("Sagutin Gamit ang Boses", font, parse_color(CONFIG["COLORS"]["text"]), button_rect.centerx, button_rect.centery, True)

        if listening:
            draw_text("Nakikinig...", font, parse_color(CONFIG["COLORS"]["wrong"]), WIDTH//2, HEIGHT//2 + 120, True)
    else:
        draw_text(message, big_font, parse_color(CONFIG["COLORS"]["wrong"]), WIDTH//2, HEIGHT//2 - 50, True)
        draw_text(f"Final Score: {score}", big_font, parse_color(CONFIG["COLORS"]["correct"]), WIDTH//2, HEIGHT//2 + 50, True)
    pygame.display.update()

while True:
    mouse_pos = pygame.mouse.get_pos()
    draw_game(mouse_pos)
    for event in pygame.event.get():
        if event.type == pygame.QUIT:
            pygame.quit()
            sys.exit()
        if not game_over:
            if event.type == pygame.KEYDOWN:
                if event.key == pygame.K_RETURN:
                    check_answer(user_text)
                elif event.key == pygame.K_BACKSPACE:
                    user_text = user_text[:-1]
                else:
                    user_text += event.unicode
            if event.type == pygame.MOUSEBUTTONDOWN:
                button_rect = pygame.Rect(*CONFIG["LAYOUT"]["button_rect"])
                if button_rect.collidepoint(event.pos) and not listening:
                    start_listening()