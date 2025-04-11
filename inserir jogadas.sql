-- Inserção de estratégias básicas iniciais
INSERT INTO JogadasBase (EstadoTabuleiro, PosicaoEscolhida, Peso)
VALUES 
-- Jogadas ofensivas (alto peso)
('X,X, , , , , , , ', 2, 100),
(' , , ,X,X, , , , ', 5, 100),
(' , , , , , ,X,X, ', 8, 100),
-- Jogadas defensivas (peso médio)
('O,O, , , , , , , ', 2, 80),
(' , ,O, , ,O, , , ', 7, 80),
-- Estratégias de centro/canto
(' , , , ,X, , , , ', 0, 30),
(' , , , ,X, , , , ', 2, 30),
(' , , , ,X, , , , ', 6, 30),
(' , , , ,X, , , , ', 8, 30);