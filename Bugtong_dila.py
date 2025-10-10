import pygame
import sys
import random
import speech_recognition as sr

pygame.init()

WIDTH, HEIGHT = 1000, 700
screen = pygame.display.set_mode((WIDTH, HEIGHT))
pygame.display.set_caption("Bugtong Dila o Tongue Twister")

FONT = pygame.font.Font(None, 48)
TITLE_FONT = pygame.font.Font(None, 64)

WHITE = (255, 255, 255)
BLACK = (0, 0, 0)
GREEN = (0, 200, 0)
RED = (200, 0, 0)

clock = pygame.time.Clock()

BUGTONGDILAA = [
    "Pitumput pitong puting tupa",
    "Minimithi kong mapabilang sa piling ng mga piling Pilipino",
    "Ang relo ni Leroy ay rolex",
    "Pasko paksiw pa",
    "Pritong isda, isda'ng prito",
    "Tangina mo"
]

current_twister = random.choice(BUGTONGDILAA)
message = "SABIHIN MO NA ANG BUGTONG DILA!!"

def listen_and_check(expected):
    recognizer = sr.Recognizer()
    with sr.Microphone() as source:
        try:
            audio = recognizer.listen(source, timeout=5)
            said = recognizer.recognize_google(audio, language="tl-PH")
            return said.lower() == expected.lower(), said
        except Exception:
            return False, ""

while True:
    screen.fill(BLACK)

    title = TITLE_FONT.render("Bugtong Dila o Tongue Twister", True, WHITE)
    screen.blit(title, (WIDTH//2 - title.get_width()//2, 50))

    twister = FONT.render(current_twister, True, WHITE)
    screen.blit(twister, (WIDTH//2 - twister.get_width()//2, HEIGHT//2 - 50))

    status = FONT.render(message, True, GREEN if "Correct" in message else RED if "Wrong" in message else WHITE)
    screen.blit(status, (WIDTH//2 - status.get_width()//2, HEIGHT - 100))

    pygame.display.flip()

    for event in pygame.event.get():
        if event.type == pygame.QUIT:
            pygame.quit()
            sys.exit()
        if event.type == pygame.KEYDOWN:
            if event.key == pygame.K_SPACE:
                correct, heard = listen_and_check(current_twister)
                if correct:
                    message = "TAMA!!"
                    pygame.display.flip()
                    pygame.time.wait(2000)
                    current_twister = random.choice(BUGTONGDILAA)
                    message = "SABIHIN MO NA ANG BUGTONG DILA!!"
                else:
                    message = f"Mali!: {heard}" if heard else "Pauulit Pwede?"

    clock.tick(30)
    