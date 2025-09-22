import pygame
import sys

pygame.init()

WIDTH, HEIGHT = 600, 600
LINE_WIDTH = 15
BOARD_ROWS = 3
BOARD_COLS = 3
SQUARE_SIZE = WIDTH // BOARD_COLS
CIRCLE_RADIUS = SQUARE_SIZE // 3
CIRCLE_WIDTH = 15
CROSS_WIDTH = 25
SPACE = SQUARE_SIZE // 4

RED = (255, 0, 0)
BG_COLOR = (28, 170, 156)
LINE_COLOR = (23, 145, 135)
CIRCLE_COLOR = (239, 231, 200)
CROSS_COLOR = (66, 66, 66)
WHITE = (255, 255, 255)

screen = pygame.display.set_mode((WIDTH, HEIGHT))
pygame.display.set_caption('XOX Game')
screen.fill(BG_COLOR)

board = [[0]*BOARD_COLS for _ in range(BOARD_ROWS)]
font = pygame.font.Font(None, 80)

def draw_lines():
    for i in range(1, BOARD_ROWS):
        pygame.draw.line(screen, LINE_COLOR, (0, i * SQUARE_SIZE), (WIDTH, i * SQUARE_SIZE), LINE_WIDTH)
        pygame.draw.line(screen, LINE_COLOR, (i * SQUARE_SIZE, 0), (i * SQUARE_SIZE, HEIGHT), LINE_WIDTH)

def draw_figures():
    for row in range(BOARD_ROWS):
        for col in range(BOARD_COLS):
            if board[row][col] == 1:
                pygame.draw.circle(screen, CIRCLE_COLOR, (int(col * SQUARE_SIZE + SQUARE_SIZE/2), int(row * SQUARE_SIZE + SQUARE_SIZE/2)), CIRCLE_RADIUS, CIRCLE_WIDTH)
            elif board[row][col] == 2:
                pygame.draw.line(screen, CROSS_COLOR, (col * SQUARE_SIZE + SPACE, row * SQUARE_SIZE + SQUARE_SIZE - SPACE), (col * SQUARE_SIZE + SQUARE_SIZE - SPACE, row * SQUARE_SIZE + SPACE), CROSS_WIDTH)
                pygame.draw.line(screen, CROSS_COLOR, (col * SQUARE_SIZE + SPACE, row * SQUARE_SIZE + SPACE), (col * SQUARE_SIZE + SQUARE_SIZE - SPACE, row * SQUARE_SIZE + SQUARE_SIZE - SPACE), CROSS_WIDTH)

def mark_square(row, col, player):
    board[row][col] = player

def available_square(row, col):
    return board[row][col] == 0

def is_board_full():
    for row in board:
        if 0 in row:
            return False
    return True

def check_win(player):
    for col in range(BOARD_COLS):
        if board[0][col] == board[1][col] == board[2][col] == player:
            return True
    for row in range(BOARD_ROWS):
        if board[row][0] == board[row][1] == board[row][2] == player:
            return True
    if board[0][0] == board[1][1] == board[2][2] == player:
        return True
    if board[0][2] == board[1][1] == board[2][0] == player:
        return True
    return False

def restart():
    global board, player, game_over
    board = [[0]*BOARD_COLS for _ in range(BOARD_ROWS)]
    screen.fill(BG_COLOR)
    draw_lines()
    player = 1
    game_over = False

def display_message(text):
    msg = font.render(text, True, WHITE)
    rect = msg.get_rect(center=(WIDTH//2, HEIGHT//2))
    screen.blit(msg, rect)
    pygame.display.update()
    pygame.time.wait(2000)

draw_lines()
player = 1
game_over = False

while True:
    for event in pygame.event.get():
        if event.type == pygame.QUIT:
            pygame.quit()
            sys.exit()
        if event.type == pygame.MOUSEBUTTONDOWN and not game_over:
            mouseX = event.pos[0] // SQUARE_SIZE
            mouseY = event.pos[1] // SQUARE_SIZE

            if available_square(mouseY, mouseX):
                mark_square(mouseY, mouseX, player)
                if check_win(player):
                    draw_figures()
                    pygame.display.update()
                    display_message(f"Player {player} Wins!")
                    game_over = True
                elif is_board_full():
                    draw_figures()
                    pygame.display.update()
                    display_message("Draw!")
                    restart()
                player = 2 if player == 1 else 1
    draw_figures()
    pygame.display.update()