angular.module('jogoDaVelhaApp').controller('VisualizarController', 
    ['$interval', 'ApiService', function($interval, ApiService) {
        var vm = this;
        
        vm.jogadas = [];
        vm.carregando = true;
        
        vm.init = function() {
            carregarJogadas();
            // Atualiza a cada 5 segundos (reduzido de 2 para melhor performance)
            vm.intervalPromise = $interval(carregarJogadas, 5000);
        };
        
        function carregarJogadas() {
            ApiService.getAprendizado().then(function(response) {
                vm.jogadas = response.data;
                vm.carregando = false;
            }).catch(function(error) {
                console.error("Erro ao carregar jogadas:", error);
                vm.carregando = false;
            });
        }
        
        vm.formatarTabuleiro = function(estado) {
            if (!estado) return '';
            var celulas = estado.split(',');

            var html = '<div class="mini-tabuleiro">';
            for (var i = 0; i < 9; i++) {
                var valor = celulas[i] || '&nbsp;';
                var classe = valor === 'X' ? 'x' : (valor === 'O' ? 'o' : '');
                html += `<div class="mini-celula ${classe}">${valor}</div>`;
            }
            html += '</div>';
            return html;
        };
        
        // Limpa o interval quando o controller é destruído
        vm.$onDestroy = function() {
            if (vm.intervalPromise) {
                $interval.cancel(vm.intervalPromise);
            }
        };
        
        vm.init();
    }]);