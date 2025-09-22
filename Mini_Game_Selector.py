import pygame
import sys
import subprocess
import time
import random

pygame.init()

WIDTH, HEIGHT = 1000, 700
screen = pygame.display.set_mode((WIDTH, HEIGHT))
pygame.display.set_caption("Mini Game Selector")

emoji_font = pygame.font.SysFont("Segoe UI Emoji", 120)
title_font = pygame.font.Font(None, 70)

WHITE = (255, 255, 255)
BLACK = (0, 0, 0)
clock = pygame.time.Clock()

CHEST_CLOSED = "🎁"
CHEST_HALF = "🤩"
CHEST_OPEN = "🥳"

menu_options = ["minigame1", "minigame2", "minigame3", "minigame4"]

games = [
    "Guess_the_Word.exe",
    "matching_cards.exe",
    "Bugtong_dila.exe",
    "XOX_game.exe"
]
random.shuffle(games)

def draw_menu(mouse_pos):
    screen.fill(BLACK)
    title = title_font.render("Choose Chest", True, WHITE)
    screen.blit(title, (WIDTH//2 - title.get_width()//2, 80))

    option_rects = []
    spacing = 200
    start_x = WIDTH//2 - ((len(menu_options)-1) * spacing)//2

    for i, option in enumerate(menu_options):
        rect = pygame.Rect(0, 0, 150, 150)
        rect.center = (start_x + i*spacing, HEIGHT // 2)

        if rect.collidepoint(mouse_pos):
            chest_symbol = CHEST_HALF
        else:
            chest_symbol = CHEST_CLOSED

        chest_text = emoji_font.render(chest_symbol, True, WHITE)
        text_rect = chest_text.get_rect(center=rect.center)
        screen.blit(chest_text, text_rect)
        option_rects.append(rect)

    pygame.display.flip()
    return option_rects

def chest_open_effect(center_pos):
    frames = [CHEST_CLOSED, CHEST_HALF, CHEST_OPEN]
    for frame in frames:
        screen.fill(BLACK)
        title = title_font.render("Choose Chest", True, WHITE)
        screen.blit(title, (WIDTH//2 - title.get_width()//2, 80))

        chest_text = emoji_font.render(frame, True, WHITE)
        rect = chest_text.get_rect(center=center_pos)
        screen.blit(chest_text, rect)
        pygame.display.flip()
        time.sleep(0.4)

while True:
    mouse_pos = pygame.mouse.get_pos()
    option_rects = draw_menu(mouse_pos)

    for event in pygame.event.get():
        if event.type == pygame.QUIT:
            pygame.quit()
            sys.exit()
        elif event.type == pygame.MOUSEBUTTONDOWN and event.button == 1:
            for i, rect in enumerate(option_rects):
                if rect.collidepoint(mouse_pos):
                    chest_open_effect(rect.center)
                    pygame.quit()
                    subprocess.run([games[i]])
                    sys.exit()

    clock.tick(60)
