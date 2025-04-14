// Substitua todo o conteúdo do JogarController por:
angular.module('jogoDaVelhaApp').controller('JogarController', 
    ['$scope', '$timeout', 'ApiService', function($scope, $timeout, ApiService) {
        var vm = this;
        
        vm.tabuleiro = ["", "", "", "", "", "", "", "", ""];
        vm.jogadorAtual = "X"; // Humano começa
        vm.mensagem = "";
        vm.carregando = false;
        
        vm.jogar = function(posicao) {
            if (vm.tabuleiro[posicao] !== "" || vm.carregando || vm.jogadorAtual !== 'X') return;
            
            // Jogada do humano
            vm.tabuleiro[posicao] = 'X';
            
            if (verificarFimDeJogo()) {
                vm.mensagem = "Você venceu!";
                $timeout(vm.resetarJogo, 2000);
                return;
            }
            
            if (verificarEmpate()) {
                vm.mensagem = "Empate!";
                $timeout(vm.resetarJogo, 2000);
                return;
            }
            
            // Troca para IA
            vm.jogadorAtual = 'O';
            vm.carregando = true;
            
            // IA joga após um delay
            $timeout(function() {
                jogadaIA();
            }, 800);
        };
        
        function jogadaIA() {
            ApiService.postJogarComBase(vm.tabuleiro.join(",")).then(function(response) {
                var posicaoIA = response.data.PosicaoEscolhida;
                
                if (posicaoIA !== -1 && vm.tabuleiro[posicaoIA] === "") {
                    vm.tabuleiro[posicaoIA] = 'O';
                    
                    if (verificarFimDeJogo()) {
                        vm.mensagem = "A IA venceu!";
                        $timeout(vm.resetarJogo, 2000);
                    } else if (verificarEmpate()) {
                        vm.mensagem = "Empate!";
                        $timeout(vm.resetarJogo, 2000);
                    } else {
                        vm.jogadorAtual = 'X'; // Volta para o humano
                    }
                }
            }).catch(function(error) {
                console.error("Erro na IA:", error);
                vm.mensagem = "A IA está com problemas. Tente novamente.";
            }).finally(function() {
                vm.carregando = false;
            });
        }
        
        function verificarFimDeJogo() {
            var linhas = [
                [0, 1, 2], [3, 4, 5], [6, 7, 8],
                [0, 3, 6], [1, 4, 7], [2, 5, 8],
                [0, 4, 8], [2, 4, 6]
            ];
            
            for (var i = 0; i < linhas.length; i++) {
                var [a, b, c] = linhas[i];
                if (vm.tabuleiro[a] && vm.tabuleiro[a] === vm.tabuleiro[b] && vm.tabuleiro[a] === vm.tabuleiro[c]) {
                    return true;
                }
            }
            return false;
        }
        
        function verificarEmpate() {
            return !vm.tabuleiro.includes("") && !verificarFimDeJogo();
        }
        
        vm.resetarJogo = function() {
            vm.tabuleiro = ["", "", "", "", "", "", "", "", ""];
            vm.jogadorAtual = "X";
            vm.mensagem = "";
        };
    }]);