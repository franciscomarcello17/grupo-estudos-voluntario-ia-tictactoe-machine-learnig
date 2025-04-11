angular.module('jogoDaVelhaApp').controller('VisualizarController', 
    ['$interval', 'ApiService', function($interval, ApiService) {
        var vm = this;
        
        vm.jogadas = [];
        vm.carregando = true;
        
        vm.init = function() {
            carregarJogadas();
            $interval(carregarJogadas, 2000);
        };
        
        function carregarJogadas() {
            ApiService.getAprendizado().then(function(response) {
                vm.jogadas = response.data;
                vm.carregando = false;
            });
        }
        
        vm.formatarTabuleiro = function(estado) {
            var celulas = estado.split(',');
            var html = '<div class="mini-tabuleiro">';
            
            for (var i = 0; i < 9; i++) {
                var classe = celulas[i].trim() === 'X' ? 'x' : (celulas[i].trim() === 'O' ? 'o' : '');
                html += `<div class="mini-celula ${classe}">${celulas[i] || '&nbsp;'}</div>`;
                if ((i + 1) % 3 === 0 && i < 8) html += '<br>';
            }
            
            html += '</div>';
            return html;
        };
        
        vm.init();
    }]);