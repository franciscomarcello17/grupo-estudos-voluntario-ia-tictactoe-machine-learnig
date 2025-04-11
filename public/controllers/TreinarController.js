angular.module('jogoDaVelhaApp').controller('TreinarController', 
    ['$scope', '$interval', 'ApiService', function($scope, $interval, ApiService) {
        var vm = this;
        
        // Estado inicial
        vm.tabuleiro = ["", "", "", "", "", "", "", "", ""];
        vm.jogadorAtual = "X";
        vm.jogadasAprendidas = [];
        vm.carregando = false;
        vm.mensagem = "";
        
        // Inicialização
        vm.init = function() {
            carregarJogadas();
            // Atualizar a cada 5 segundos
            $interval(carregarJogadas, 5000);
        };
        
        function carregarJogadas() {
            ApiService.getAprendizado().then(function(response) {
                vm.jogadasAprendidas = response.data;
            });
        }
        
        // Lógica do jogo
        vm.jogar = function(posicao) {
            if (vm.tabuleiro[posicao] !== "" || vm.carregando) return;
            
            // Jogada do humano
            vm.tabuleiro[posicao] = vm.jogadorAtual;
            
            // Verificar fim de jogo
            if (verificarFimDeJogo()) {
                registrarJogada("Vitoria");
                return;
            }
            
            // Trocar jogador
            vm.jogadorAtual = vm.jogadorAtual === "X" ? "O" : "X";
            vm.carregando = true;
            
            // Jogada da IA
            ApiService.postTreinar({
                EstadoTabuleiro: vm.tabuleiro.join(","),
                PosicaoEscolhida: posicao,
                Resultado: ""
            }).then(function() {
                // IA faz sua jogada
                return ApiService.postJogarComBase(vm.tabuleiro.join(","));
            }).then(function(response) {
                var posicaoIA = response.data.PosicaoEscolhida;
                
                if (posicaoIA !== -1 && vm.tabuleiro[posicaoIA] === "") {
                    vm.tabuleiro[posicaoIA] = vm.jogadorAtual;
                    
                    if (verificarFimDeJogo()) {
                        registrarJogada("Derrota");
                    }
                }
                
                vm.jogadorAtual = vm.jogadorAtual === "X" ? "O" : "X";
            }).catch(function(error) {
                vm.mensagem = "Erro ao comunicar com a IA: " + error.data;
            }).finally(function() {
                vm.carregando = false;
            });
        };
        
        function verificarFimDeJogo() {
            // Lógica de verificação de vitória/empate
            var linhas = [
                [0, 1, 2], [3, 4, 5], [6, 7, 8], // linhas
                [0, 3, 6], [1, 4, 7], [2, 5, 8], // colunas
                [0, 4, 8], [2, 4, 6]             // diagonais
            ];
            
            for (var i = 0; i < linhas.length; i++) {
                var [a, b, c] = linhas[i];
                if (vm.tabuleiro[a] && vm.tabuleiro[a] === vm.tabuleiro[b] && vm.tabuleiro[a] === vm.tabuleiro[c]) {
                    return true;
                }
            }
            
            return !vm.tabuleiro.includes("");
        }
        
        function registrarJogada(resultado) {
            var jogada = {
                EstadoTabuleiro: vm.tabuleiro.join(","),
                PosicaoEscolhida: -1,
                Resultado: resultado
            };
            
            ApiService.postTreinar(jogada).then(function() {
                vm.mensagem = "Jogo finalizado! Resultado: " + resultado;
                setTimeout(function() {
                    vm.resetarJogo();
                    $scope.$apply();
                }, 2000);
            });
        }
        
        vm.resetarJogo = function() {
            vm.tabuleiro = ["", "", "", "", "", "", "", "", ""];
            vm.jogadorAtual = "X";
            vm.mensagem = "";
        };
        
        vm.limparIA = function() {
            if (confirm("Tem certeza que deseja limpar todo o aprendizado da IA?")) {
                ApiService.deleteAprendizado().then(function() {
                    vm.jogadasAprendidas = [];
                    vm.mensagem = "IA resetada com sucesso!";
                });
            }
        };
        
        vm.init();
    }]);