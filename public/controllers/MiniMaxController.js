angular.module('jogoDaVelhaApp')
.controller('MiniMaxController', ['$sce', function($sce) {
    const vm = this;
    
    // Estado inicial do jogo
    vm.board = [
        [null, null, null],
        [null, null, null],
        [null, null, null]
    ];
    vm.currentPlayer = 'X';
    vm.gameOver = false;
    vm.winner = null;
    vm.userInput = '';
    vm.terminalLines = [];
    vm.showBoard = false; // Novo controle para exibir o tabuleiro
    vm.treeVisualization = $sce.trustAsHtml('Árvore de decisão será exibida aqui após sua jogada...');
    
    // Mapeamento de entrada do usuário para coordenadas
    const inputMap = {
        'a1': [0, 0], 'a2': [1, 0], 'a3': [2, 0],
        'b1': [0, 1], 'b2': [1, 1], 'b3': [2, 1],
        'c1': [0, 2], 'c2': [1, 2], 'c3': [2, 2]
    };
    
    // Função para adicionar linha ao terminal (definida antes de ser usada)
    vm.addTerminalLine = function(text) {
        vm.terminalLines.push(text);
        // Scroll automático para o final
        setTimeout(() => {
            const output = document.querySelector('.cmd-output');
            if (output) output.scrollTop = output.scrollHeight;
        }, 10);
    };
    
    // Inicializa o terminal com mensagem de boas-vindas
    
    vm.addTerminalLine('Bem-vindo ao Jogo da Velha com IA MiniMax!');
    vm.addTerminalLine('Digite as coordenadas (ex: a1, b2, c3) para jogar');
    vm.addTerminalLine('Digite "help" para ver os comandos disponíveis');
    vm.addTerminalLine('Digite "cls" para limpar o console e começar');

    
    // Manipula a entrada do usuário
    vm.handleInput = function(event) {
        if (event.keyCode === 13) { // Enter key
            const input = vm.userInput.toLowerCase().trim();
            
            // Adiciona o comando ao histórico
            vm.addTerminalLine('C:\\Users\\Player: ' + input);
            
            // Comandos do console
            if (input === 'cls') {
                vm.terminalLines = [];
                vm.showBoard = true;
                vm.addTerminalLine('Console limpo. Jogo iniciado!');
                vm.addTerminalLine('Você é o jogador X - Faça sua jogada (ex: a1)');
                vm.userInput = '';
                return;
            }
            
            if (input === 'reset') {
                vm.resetGame();
                vm.userInput = '';
                return;
            }
            
            if (input === 'help') {
                vm.showHelp();
                vm.userInput = '';
                return;
            }
            
            if (input === 'debug') {
                vm.addTerminalLine('Modo debug ativado');
                vm.addTerminalLine('Tabuleiro atual: ' + JSON.stringify(vm.board));
                vm.userInput = '';
                return;
            }
            
            if (!vm.showBoard) {
                vm.addTerminalLine('Digite "cls" para começar o jogo');
                vm.userInput = '';
                return;
            }
            
            if (vm.gameOver) {
                vm.addTerminalLine('Jogo terminado. Digite "reset" para jogar novamente.');
                vm.userInput = '';
                return;
            }
            
            if (inputMap[input]) {
                const [row, col] = inputMap[input];
                if (vm.board[row][col] === null) {
                    vm.makeMove(row, col, 'X');
                    
                    if (!vm.gameOver) {
                        // IA faz sua jogada
                        setTimeout(() => {
                            const bestMove = vm.findBestMove();
                            if (bestMove) {
                                vm.makeMove(bestMove.row, bestMove.col, 'O');
                            }
                        }, 700);
                    }
                } else {
                    vm.addTerminalLine('Posição já ocupada. Tente outra.');
                }
            } else {
                vm.addTerminalLine('Entrada inválida. Use formato letra+número (ex: a1, b2, c3)');
                vm.addTerminalLine('Digite "help" para ver os comandos disponíveis');
            }
            
            vm.userInput = '';
        }
    };
    
    // Mostra ajuda
    vm.showHelp = function() {
        vm.addTerminalLine('Comandos disponíveis:');
        vm.addTerminalLine('cls - Limpa o console e inicia o jogo');
        vm.addTerminalLine('reset - Reinicia o jogo');
        vm.addTerminalLine('help - Mostra esta ajuda');
        vm.addTerminalLine('debug - Mostra informações de depuração');
        vm.addTerminalLine('Coordenadas válidas: a1, a2, a3, b1, b2, b3, c1, c2, c3');
    };
    
    // Realiza uma jogada
    vm.makeMove = function(row, col, player) {
        vm.board[row][col] = player;
        const move = String.fromCharCode(97 + col) + (row + 1);
        vm.addTerminalLine(`Jogador ${player} moveu para ${move}`);
        
        // Verifica se o jogo terminou
        vm.checkGameStatus();
        
        // Atualiza a visualização da árvore
        if (player === 'X') {
            vm.updateTreeVisualization();
        }
    };
    
    // Verifica o status do jogo (vitória/empate)
    vm.checkGameStatus = function() {
        const lines = [
            // Linhas horizontais
            [[0, 0], [0, 1], [0, 2]],
            [[1, 0], [1, 1], [1, 2]],
            [[2, 0], [2, 1], [2, 2]],
            // Linhas verticais
            [[0, 0], [1, 0], [2, 0]],
            [[0, 1], [1, 1], [2, 1]],
            [[0, 2], [1, 2], [2, 2]],
            // Diagonais
            [[0, 0], [1, 1], [2, 2]],
            [[0, 2], [1, 1], [2, 0]]
        ];
        
        // Verifica vitória
        for (let line of lines) {
            const [a, b, c] = line;
            if (vm.board[a[0]][a[1]] && 
                vm.board[a[0]][a[1]] === vm.board[b[0]][b[1]] && 
                vm.board[a[0]][a[1]] === vm.board[c[0]][c[1]]) {
                vm.winner = vm.board[a[0]][a[1]];
                vm.gameOver = true;
                vm.addTerminalLine(`Jogador ${vm.winner} venceu!`);
                return;
            }
        }
        
        // Verifica empate
        let isDraw = true;
        for (let row = 0; row < 3; row++) {
            for (let col = 0; col < 3; col++) {
                if (vm.board[row][col] === null) {
                    isDraw = false;
                    break;
                }
            }
            if (!isDraw) break;
        }
        
        if (isDraw) {
            vm.gameOver = true;
            vm.addTerminalLine('Empate!');
        }
    };
    
    // Algoritmo MiniMax
    vm.minimax = function(board, depth, isMaximizing) {
        const score = vm.evaluateBoard(board);
        if (score !== 0) return score;
        if (vm.isBoardFull(board)) return 0;
        
        if (isMaximizing) {
            let bestScore = -Infinity;
            for (let row = 0; row < 3; row++) {
                for (let col = 0; col < 3; col++) {
                    if (board[row][col] === null) {
                        board[row][col] = 'O';
                        const score = vm.minimax(board, depth + 1, false);
                        board[row][col] = null;
                        bestScore = Math.max(score, bestScore);
                    }
                }
            }
            return bestScore;
        } else {
            let bestScore = Infinity;
            for (let row = 0; row < 3; row++) {
                for (let col = 0; col < 3; col++) {
                    if (board[row][col] === null) {
                        board[row][col] = 'X';
                        const score = vm.minimax(board, depth + 1, true);
                        board[row][col] = null;
                        bestScore = Math.min(score, bestScore);
                    }
                }
            }
            return bestScore;
        }
    };
    
    // Avalia o tabuleiro
    vm.evaluateBoard = function(board) {
        const lines = [
            [[0, 0], [0, 1], [0, 2]],
            [[1, 0], [1, 1], [1, 2]],
            [[2, 0], [2, 1], [2, 2]],
            [[0, 0], [1, 0], [2, 0]],
            [[0, 1], [1, 1], [2, 1]],
            [[0, 2], [1, 2], [2, 2]],
            [[0, 0], [1, 1], [2, 2]],
            [[0, 2], [1, 1], [2, 0]]
        ];
        
        for (let line of lines) {
            const [a, b, c] = line;
            if (board[a[0]][a[1]] && 
                board[a[0]][a[1]] === board[b[0]][b[1]] && 
                board[a[0]][a[1]] === board[c[0]][c[1]]) {
                return board[a[0]][a[1]] === 'O' ? 1 : -1;
            }
        }
        return 0;
    };
    
    // Verifica se o tabuleiro está cheio
    vm.isBoardFull = function(board) {
        for (let row = 0; row < 3; row++) {
            for (let col = 0; col < 3; col++) {
                if (board[row][col] === null) return false;
            }
        }
        return true;
    };
    
    // Encontra a melhor jogada para a IA
    vm.findBestMove = function() {
        let bestScore = -Infinity;
        let bestMove = null;
        
        for (let row = 0; row < 3; row++) {
            for (let col = 0; col < 3; col++) {
                if (vm.board[row][col] === null) {
                    vm.board[row][col] = 'O';
                    const score = vm.minimax(vm.board, 0, false);
                    vm.board[row][col] = null;
                    
                    if (score > bestScore) {
                        bestScore = score;
                        bestMove = { row, col };
                    }
                }
            }
        }
        
        return bestMove;
    };
    
    // Atualiza a visualização da árvore de decisão
    vm.updateTreeVisualization = function() {
        let tree = 'Raiz [Jogada atual]\n';
        const bestMove = vm.findBestMove();
        
        // Simula uma árvore de decisão
        for (let row = 0; row < 3; row++) {
            for (let col = 0; col < 3; col++) {
                if (vm.board[row][col] === null) {
                    const move = String.fromCharCode(97 + col) + (row + 1);
                    // Dentro do loop em updateTreeVisualization()
                    vm.board[row][col] = 'O';  // Simula jogada da IA
                    const score = vm.minimax(vm.board, 0, false);  // Obtém avaliação real
                    vm.board[row][col] = null;  // Desfaz simulação                    
                    if (bestMove && bestMove.row === row && bestMove.col === col) {
                        tree += `└─ <span class="node-selected">${move} [Avaliação: ${score}] ← Melhor jogada</span>\n`;
                    } else {
                        tree += `├─ ${move} [Avaliação: ${score}]\n`;
                    }
                    
                    // Adiciona alguns nós filhos de exemplo
                    if (row === bestMove?.row && col === bestMove?.col) {
                        tree += `│  ├─ <span class="node-value">Valor: ${score}</span>\n`;
                        tree += `│  └─ <span class="node-move">Profundidade: 3</span>\n`;
                    }
                }
            }
        }
        
        vm.treeVisualization = $sce.trustAsHtml(tree);
    };
    
    // Reinicia o jogo
    vm.resetGame = function() {
        vm.board = [
            [null, null, null],
            [null, null, null],
            [null, null, null]
        ];
        vm.currentPlayer = 'X';
        vm.gameOver = false;
        vm.winner = null;
        vm.showBoard = true;
        vm.terminalLines = [];
        vm.treeVisualization = $sce.trustAsHtml('Árvore de decisão será exibida aqui após sua jogada...');
        vm.addTerminalLine('Jogo reiniciado. Você é o jogador X');
        vm.addTerminalLine('Digite coordenadas (ex: a1, b2, c3) para jogar');
    };
}])
.filter('trustHtml', ['$sce', function($sce) {
    return function(html) {
        return $sce.trustAsHtml(html);
    };
}]);