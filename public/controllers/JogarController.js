angular.module('jogoDaVelhaApp').controller('JogarController', 
    ['$scope', 'ApiService', function($scope, ApiService) {
        var vm = this;
        
        vm.tabuleiro = ["", "", "", "", "", "", "", "", ""];
        vm.jogadorAtual = "X";
        vm.mensagem = "";
        vm.carregando = false;
        
        vm.jogar = function(posicao) {
            if (vm.tabuleiro[posicao] !== "" || vm.carregando) return;
            
            // Jogada do humano
            vm.tabuleiro[posicao] = vm.jogadorAtual;
            
            if (verificarFimDeJogo()) {
                vm.mensagem = "Você venceu!";
                setTimeout(vm.resetarJogo, 2000);
                return;
            }
            
            vm.carregando = true;
            
            // Jogada da IA
            ApiService.postJogarComBase(vm.tabuleiro.join(",")).then(function(response) {
                var posicaoIA = response.data.PosicaoEscolhida;
                
                if (posicaoIA !== -1 && vm.tabuleiro[posicaoIA] === "") {
                    vm.tabuleiro[posicaoIA] = "O";
                    
                    if (verificarFimDeJogo()) {
                        vm.mensagem = "A IA venceu!";
                        setTimeout(vm.resetarJogo, 2000);
                    }
                }
            }).catch(function(error) {
                vm.mensagem = "Erro ao jogar contra a IA: " + error.data;
            }).finally(function() {
                vm.carregando = false;
            });
        };
        
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
            
            return !vm.tabuleiro.includes("");
        }
        
        vm.resetarJogo = function() {
            vm.tabuleiro = ["", "", "", "", "", "", "", "", ""];
            vm.jogadorAtual = "X";
            vm.mensagem = "";
        };
    }]);