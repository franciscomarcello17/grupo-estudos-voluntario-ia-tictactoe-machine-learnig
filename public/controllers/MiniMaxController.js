angular.module('jogoDaVelhaApp')
.controller('MiniMaxController', ['$sce', '$window', function($sce, $window) {
    const vm = this;
    
    // Estado inicial do jogo
    vm.board = [
        [null, null, null],
        [null, null, null],
        [null, null, null]
    ];
    vm.currentPlayer = 'X';
    vm.gameOver = false;
    vm.isGameStarted = false;
    vm.winner = null;
    vm.userInput = '';
    vm.terminalLines = [];
    vm.showBoard = false;
    vm.treeVisualization = $sce.trustAsHtml('Árvore de decisão será exibida aqui após sua jogada...');
    vm.commandHistory = [];
    vm.historyIndex = -1;
    vm.tempInput = '';
    vm.waitingForEnter = false;

    // Mapeamento de entrada do usuário para coordenadas
    const inputMap = {
        'a1': [0, 0], 'a2': [1, 0], 'a3': [2, 0],
        'b1': [0, 1], 'b2': [1, 1], 'b3': [2, 1],
        'c1': [0, 2], 'c2': [1, 2], 'c3': [2, 2]
    };
    
    // Função para adicionar linha ao terminal
    vm.addTerminalLine = function(text, isWarning = false) {
        if (isWarning) {
            const trustedHtml = $sce.trustAsHtml(`<span class="warning-text">${text}</span>`);
            vm.terminalLines.push(trustedHtml);
        } else {
            vm.terminalLines.push(text);
        }
        setTimeout(() => {
            const output = document.querySelector('.cmd-output');
            if (output) output.scrollTop = output.scrollHeight;
        }, 10);
    };
    
    // Inicializa o terminal com mensagem de boas-vindas
    vm.addTerminalLine('Bem-vindo ao Jogo da Velha com MiniMax!');
    vm.addTerminalLine('Digite as coordenadas (ex: a1, b2, c3) para jogar');
    vm.addTerminalLine('Digite "cls" para limpar o console');
    vm.addTerminalLine('Digite "help" para ver os comandos disponíveis');
    vm.addTerminalLine('Digite "start" para iniciar o jogo');
    
    // Manipula a entrada do usuário
    vm.handleInput = function(event) {
        if (event.keyCode === 13) { // Enter key
            const input = vm.userInput.trim();
            
            if (input) {
                vm.commandHistory.unshift(input);
                if (vm.commandHistory.length > 50) {
                    vm.commandHistory.pop();
                }
            }
            vm.historyIndex = -1;
            
            vm.processCommand(input);
            vm.userInput = '';
        } else if (event.keyCode === 38) { // Seta para cima
            event.preventDefault();
            vm.navigateHistory('up');
        } else if (event.keyCode === 40) { // Seta para baixo
            event.preventDefault();
            vm.navigateHistory('down');
        }
    };
    
    // Navega pelo histórico de comandos
    vm.navigateHistory = function(direction) {
        if (vm.commandHistory.length === 0) return;
        
        if (direction === 'up') {
            if (vm.historyIndex < vm.commandHistory.length - 1) {
                if (vm.historyIndex === -1) {
                    vm.tempInput = vm.userInput;
                }
                vm.historyIndex++;
                vm.userInput = vm.commandHistory[vm.historyIndex];
            }
        } else if (direction === 'down') {
            if (vm.historyIndex > 0) {
                vm.historyIndex--;
                vm.userInput = vm.commandHistory[vm.historyIndex];
            } else if (vm.historyIndex === 0) {
                vm.historyIndex = -1;
                vm.userInput = vm.tempInput || '';
            }
        }
    };
    
    // Processa o comando digitado
    vm.processCommand = function(input) {
        const lowerInput = input.toLowerCase();
        
        if (!['cls', 'reset', 'exit'].includes(lowerInput)) {
            vm.addTerminalLine('C:\\Users\\Player> ' + input);
        }
        
        if (input === 'start') {
            if (vm.isGameStarted) {
                vm.addTerminalLine('O jogo já está em andamento. Digite "reset" para reiniciar.');
                vm.userInput = '';
                return;
            }
            vm.startGame();
            vm.userInput = '';
            return;
        }
        
        if (input === 'cls') {
            vm.clearConsole();
            vm.userInput = '';
            return;
        }
        
        if (input === 'coords') {
            vm.addTerminalLine('Coordenadas válidas: a1, a2, a3, b1, b2, b3, c1, c2, c3', true);
        }

        if (input === 'reset') {
            vm.resetGame();
            vm.userInput = '';
            return;
        }
        
        if (lowerInput === 'exit') {
            vm.closeMinimax();
            return;
        }
        
        if (lowerInput === 'help') {
            vm.showHelp();
            return;
        }
        
        if (!vm.showBoard) {
            vm.addTerminalLine('Digite "start" para começar o jogo');
            return;
        }
        
        if (vm.gameOver) {
            vm.addTerminalLine('Jogo terminado. Digite "reset" para jogar novamente.');
            return;
        }
        
        if (vm.waitingForEnter) {
            vm.waitingForEnter = false;
            const bestMove = vm.findBestMove();
            if (bestMove) {
                const iaMove = String.fromCharCode(97 + bestMove.col) + (bestMove.row + 1);
                vm.addTerminalLine('C:\\Users\\MiniMax> ' + iaMove);
                vm.makeMove(bestMove.row, bestMove.col, 'O');
            }
            return;
        }
        
        const normalizedInput = lowerInput;
        if (inputMap[normalizedInput]) {
            const [row, col] = inputMap[normalizedInput];
            if (vm.board[row][col] === null) {
                vm.makeMove(row, col, 'X');
                vm.addTerminalLine($sce.trustAsHtml('Pressione ENTER novamente para a IA fazer sua jogada'), true);
                vm.waitingForEnter = true;
            } else {
                vm.addTerminalLine('Posição já ocupada. Tente outra.');
            }
        } else {
            vm.addTerminalLine('Entrada inválida. Use formato letra+número (ex: a1, b2, c3)');
            vm.addTerminalLine('Digite "help" para ver os comandos disponíveis');
        }
    };
        
    vm.startGame = function() {
        vm.showBoard = true;
        vm.isGameStarted = true;
        vm.addTerminalLine('Jogo iniciado!');
        vm.addTerminalLine('Você é o jogador X - Faça sua jogada (ex: a1)');
    };

    vm.clearConsole = function() {
        vm.terminalLines = [];
    };
    
    // Mostra ajuda
    vm.showHelp = function() {
        vm.addTerminalLine('Comandos disponíveis:', true);
        vm.addTerminalLine('cls - Limpa o console');
        vm.addTerminalLine('coords - Mostra as coordenadas válidas para jogar');
        vm.addTerminalLine('exit - Volta para a página anterior');
        vm.addTerminalLine('help - Mostra menu de ajuda');
        vm.addTerminalLine('reset - Reinicia o jogo');
        vm.addTerminalLine('start - Inicia o jogo');
    };
    
    // Realiza uma jogada (sem mostrar a mensagem de jogada)
    vm.makeMove = function(row, col, player) {
        vm.board[row][col] = player;
        vm.checkGameStatus();
        
        if (player === 'X') {
            vm.updateTreeVisualization();
        }
    };
    
    // Verifica o status do jogo (vitória/empate)
    vm.checkGameStatus = function() {
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
            if (vm.board[a[0]][a[1]] && 
                vm.board[a[0]][a[1]] === vm.board[b[0]][b[1]] && 
                vm.board[a[0]][a[1]] === vm.board[c[0]][c[1]]) {
                vm.winner = vm.board[a[0]][a[1]];
                vm.gameOver = true;
                vm.addTerminalLine(`Jogador ${vm.winner} venceu!`);
                return;
            }
        }
        
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
    
    // Funções para a árvore de decisão
    vm.treeData = [];
    vm.expandedNodes = {};
    
    vm.updateTreeVisualization = function() {
        vm.treeData = vm.generateTreeData(vm.board, 'O', 0, 3);
        vm.renderTree();
    };
    
    vm.generateTreeData = function(board, player, depth, maxDepth) {
        if (depth >= maxDepth) return null;
        
        const nodes = [];
        const nextPlayer = player === 'O' ? 'X' : 'O';
        
        for (let row = 0; row < 3; row++) {
            for (let col = 0; col < 3; col++) {
                if (board[row][col] === null) {
                    const move = String.fromCharCode(97 + col) + (row + 1);
                    
                    board[row][col] = player;
                    const score = vm.minimax(board, depth + 1, player === 'X');
                    const children = depth < maxDepth - 1 ? vm.generateTreeData(board, nextPlayer, depth + 1, maxDepth) : null;
                    
                    board[row][col] = null;
                    
                    nodes.push({
                        id: `${move}-${depth}`,
                        move: move,
                        player: player,
                        score: score,
                        depth: depth,
                        children: children
                    });
                }
            }
        }
        
        return nodes.sort((a, b) => b.score - a.score);
    };
    
    vm.renderTree = function() {
        let html = '<div class="tree-root">Raiz [Jogada atual]</div>';
        html += vm.renderTreeLevel(vm.treeData, 0);
        vm.treeVisualization = $sce.trustAsHtml(html);
    };
    
    vm.renderTreeLevel = function(nodes, level) {
        if (!nodes || nodes.length === 0) return '';
        
        let html = '<ul class="tree-level">';
        const isExpanded = (node) => vm.expandedNodes[node.id];
        
        nodes.forEach((node, index) => {
            const isLast = index === nodes.length - 1;
            const prefix = level > 0 ? '│  '.repeat(level - 1) + (isLast ? '└─ ' : '├─ ') : '';
            const hasChildren = node.children && node.children.length > 0;
            
            let nodeClass = 'tree-node';
            if (node.player === 'O') nodeClass += ' node-ai';
            if (node.player === 'X') nodeClass += ' node-player';
            if (node.score === 1) nodeClass += ' node-win';
            if (node.score === -1) nodeClass += ' node-loss';
            
            html += `<li class="${nodeClass}">`;
            html += `<span class="node-prefix">${prefix}</span>`;
            html += `<span class="node-content" data-node-id="${node.id}">`;
            html += `${node.move} [${node.player}] (Avaliação: ${node.score})`;
            if (hasChildren) {
                html += ` <span class="node-toggle">[${isExpanded(node) ? '-' : '+'}]</span>`;
            }
            html += '</span>';
            
            if (hasChildren && isExpanded(node)) {
                html += vm.renderTreeLevel(node.children, level + 1);
            }
            
            html += '</li>';
        });
        
        html += '</ul>';
        return html;
    };
    
    vm.toggleNode = function(nodeId) {
        vm.expandedNodes[nodeId] = !vm.expandedNodes[nodeId];
        vm.clickSound();
        vm.renderTree();
    };    
    
    vm.clickSound = function() {
        const audio = new Audio('/assets/sounds/Windows Navigation Start.wav');
        audio.volume = 0.2;
        audio.play();
    };
    
    vm.handleTreeClick = function(event) {
        let target = event.target;
        if (target.classList.contains('node-toggle') || 
            target.classList.contains('node-content')) {
            
            if (target.classList.contains('node-toggle')) {
                target = target.parentElement;
            }
            
            const nodeId = target.getAttribute('data-node-id');
            if (nodeId) {
                vm.toggleNode(nodeId);
                event.stopPropagation();
            }
        }
    };
    
    vm.showTooltip = function(event) {
        const tooltip = event.currentTarget.querySelector('.click-tooltip');
        tooltip.classList.add('show');
        
        setTimeout(() => {
            tooltip.classList.remove('show');
        }, 1000);
        
        event.preventDefault();
        const audio = new Audio('/assets/sounds/Windows Notify System Generic.wav');
        audio.volume = 0.2;
        audio.play();
    };
    
    vm.hideTree = function() {
        vm.clickSound();
        vm.isTreeHidden = !vm.isTreeHidden;
    };
    
    vm.closeMinimax = function() {
        vm.clickSound();
        setTimeout(function() {
            $window.location.href = '/treinar';
        }, 100);
    };
    
    // Reinicia completamente o jogo
    vm.resetGame = function() {
        vm.board = [
            [null, null, null],
            [null, null, null],
            [null, null, null]
        ];
        vm.currentPlayer = 'X';
        vm.gameOver = false;
        vm.isGameStarted = false;
        vm.winner = null;
        vm.showBoard = true;
        vm.terminalLines = [];
        vm.isTreeHidden = false;
        vm.waitingForEnter = false;
        vm.treeVisualization = $sce.trustAsHtml('Árvore de decisão será exibida aqui após sua jogada...');
        vm.addTerminalLine('Bem-vindo ao Jogo da Velh-IA com IA MiniMax!');
        vm.addTerminalLine('Digite as coordenadas (ex: a1, b2, c3) para jogar');
        vm.addTerminalLine('Digite "cls" para limpar o console');
        vm.addTerminalLine('Digite "help" para ver os comandos disponíveis');
        vm.addTerminalLine('Digite "start" para iniciar o jogo');
    };
}])
.filter('trustHtml', ['$sce', function($sce) {
    return function(html) {
        return $sce.trustAsHtml(html);
    };
}]);