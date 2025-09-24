import pygame
import sys 
import os

pygame.init()
WIDTH, HEIGHT = 1200, 800
SCREEN = pygame.display.set_mode((WIDTH, HEIGHT))
pygame.display.set_caption("Character Selection")

FONT = pygame.font.SysFont("arial", 22, bold=True)
BIGFONT = pygame.font.SysFont("arial", 42, bold=True)

characters = [
    {"Name": "Darna", "energy": 30, "Buff": 10, "Bonus": 5, "img": "Characters/photo_6237997538955348045_y.jpg"},
    {"Name": "Boy Tisoy", "energy": 30, "Buff": 10, "Bonus": 4, "img": "Characters/photo_6237997538955348046_y.jpg"},
    {"Name": "Aiza", "energy": 30, "Buff": 10, "Bonus": 4, "img": "Characters/photo_6237997538955348047_y.jpg"},
    {"Name": "Manang Yolly", "energy": 30, "Buff": 10, "Bonus": 6, "img": "Characters/photo_6237997538955348048_y.jpg"},
    {"Name": "Dyesebelina", "energy": 30, "Buff": 10, "Bonus": 5, "img": "Characters/photo_6237997538955348049_y.jpg"},
    {"Name": "Diego", "energy": 30, "Buff": 10, "Bonus": 4, "img": "Characters/photo_6237997538955348050_y.jpg"},
    {"Name": "Kardo", "energy": 30, "Buff": 10, "Bonus": 5, "img": "Characters/photo_6237997538955348051_y.jpg"},
    {"Name": "Mang Berto", "energy": 30, "Buff": 10, "Bonus": 4, "img": "Characters/photo_6237997538955348052_y.jpg"},
    {"Name": "Dora", "energy": 30, "Buff": 10, "Bonus": 5, "img": "Characters/photo_6237997538955348053_y.jpg"},
    {"Name": "King Kiko", "energy": 30, "Buff": 10, "Bonus": 6, "img": "Characters/photo_6237997538955348054_y.jpg"}
]

BASE = os.path.dirname(__file__)
for c in characters:
    path = os.path.join(BASE, c["img"])
    img = pygame.image.load(path).convert_alpha()
    c["surface"] = pygame.transform.smoothscale(img, (160, 160))

selected_index = 0
selected_characters = []
hover_index = None
confirm_pressed = False

BG_COLOR = (20, 20, 30)
CARD_BG = (45, 45, 60)
HOVER_BORDER = (0, 180, 255)
SELECT_BORDER = (255, 215, 0)
DEFAULT_BORDER = (120, 120, 120)

def draw_card(c, rect, is_selected, is_hovered):
    border_color = DEFAULT_BORDER
    if is_selected: border_color = SELECT_BORDER
    if is_hovered: border_color = HOVER_BORDER

    pygame.draw.rect(SCREEN, border_color, rect, border_radius=12, width=4)
    pygame.draw.rect(SCREEN, CARD_BG, rect.inflate(-6, -6), border_radius=10)

    img_rect = c["surface"].get_rect(center=(rect.centerx, rect.y + 90))
    SCREEN.blit(c["surface"], img_rect)

    name_text = FONT.render(c["Name"], True, (255, 255, 255))
    SCREEN.blit(name_text, (rect.centerx - name_text.get_width()//2, rect.y + 180))

    if is_hovered or is_selected:
        stats = [f"Energy: {c['energy']}", f"BUFF: {c['Buff']}", f"BONUS: {c['Bonus']}"]
        for i, s in enumerate(stats):
            stat_text = FONT.render(s, True, (200, 200, 200))
            SCREEN.blit(stat_text, (rect.x + 10, rect.y + 200 + i*18))

def draw_button(text, rect, active=False):
    color = (0, 180, 80) if active else (80, 80, 80)
    pygame.draw.rect(SCREEN, color, rect, border_radius=12)
    pygame.draw.rect(SCREEN, (255, 255, 255), rect, width=2, border_radius=12)
    label = BIGFONT.render(text, True, (255, 255, 255))
    SCREEN.blit(label, (rect.centerx - label.get_width()//2, rect.centery - label.get_height()//2))

def draw():
    SCREEN.fill(BG_COLOR)
    title = BIGFONT.render("Select 4 to 5 Characters", True, (255, 255, 255))
    SCREEN.blit(title, (WIDTH//2 - title.get_width()//2, 20))

    cols, spacing_x, spacing_y = 5, 220, 280
    start_x, start_y = 60, 100

    card_rects = []
    for i, c in enumerate(characters):
        row, col = divmod(i, cols)
        x = start_x + col * spacing_x
        y = start_y + row * spacing_y
        rect = pygame.Rect(x, y, 180, 260)
        draw_card(c, rect, i in selected_characters, i == hover_index or i == selected_index)
        card_rects.append(rect)

    footer = FONT.render(f"Selected: {len(selected_characters)}/5", True, (255, 255, 0))
    SCREEN.blit(footer, (20, HEIGHT - 70))

    button_rect = pygame.Rect(WIDTH//2 - 120, HEIGHT - 80, 240, 60)
    draw_button("Confirm", button_rect, active=len(selected_characters) in range(4, 6))

    pygame.display.flip()
    return card_rects, button_rect

while True:
    hover_index = None
    card_rects, button_rect = draw()
    for event in pygame.event.get():
        if event.type == pygame.QUIT:
            pygame.quit(); sys.exit()
        if event.type == pygame.MOUSEMOTION:
            mx, my = event.pos
            for i, rect in enumerate(card_rects):
                if rect.collidepoint(mx, my):
                    hover_index = i
        if event.type == pygame.MOUSEBUTTONDOWN:
            mx, my = event.pos
            if button_rect.collidepoint(mx, my) and len(selected_characters) in range(4, 6):
                confirm_pressed = True
                print("Confirmed selection:", [characters[i]["Name"] for i in selected_characters])
            else:
                for i, rect in enumerate(card_rects):
                    if rect.collidepoint(mx, my):
                        if i in selected_characters:
                            selected_characters.remove(i)
                        elif len(selected_characters) < 5:
                            selected_characters.append(i)
        if event.type == pygame.KEYDOWN:
            if event.key == pygame.K_RIGHT:
                selected_index = (selected_index + 1) % len(characters)
            if event.key == pygame.K_LEFT:
                selected_index = (selected_index - 1) % len(characters)
            if event.key == pygame.K_DOWN:
                selected_index = (selected_index + 5) % len(characters)
            if event.key == pygame.K_UP:
                selected_index = (selected_index - 5) % len(characters)
            if event.key == pygame.K_RETURN:
                if selected_index in selected_characters:
                    selected_characters.remove(selected_index)
                elif len(selected_characters) < 5:
                    selected_characters.append(selected_index)
            if event.key == pygame.K_ESCAPE:
                pygame.quit(); sys.exit()   
    