angular.module('jogoDaVelhaApp').factory('ApiService', ['$http', function($http) {
    const baseUrl = 'https://localhost:7193/api'; // Substitua pela sua URL
    
    return {
        getAprendizado: function() {
            return $http.get(baseUrl + '/aprendizado');
        },
        
        postTreinar: function(jogada) {
            return $http.post(baseUrl + '/aprendizado/treinar', jogada);
        },
        
        deleteAprendizado: function() {
            return $http.delete(baseUrl + '/aprendizado');
        },
        
        getJogadasBase: function() {
            return $http.get(baseUrl + '/base/jogadas-base');
        },
        
        postJogarComBase: function(estadoTabuleiro) {
            return $http.post(baseUrl + '/base/jogar-com-base', {
                EstadoTabuleiro: estadoTabuleiro,
                PosicaoEscolhida: 0,
                Resultado: ""
            });
        }
    };
}]);