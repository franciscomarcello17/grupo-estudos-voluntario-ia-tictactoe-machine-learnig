// Configuração do módulo principal
angular.module('jogoDaVelhaApp', [
    'ngRoute'
])

// Configuração para desativar SCE (Strict Contextual Escaping)
.config(function($sceProvider) {
    $sceProvider.enabled(false);
})

// Filtro personalizado para ajustar a data para o fuso de Brasília
.filter('brasiliaDate', function($filter) {
    return function(utcDateString, format) {
        if (!utcDateString) return '';
        const utcDate = new Date(utcDateString);
        const brasiliaDate = new Date(utcDate.getTime() - (3 * 60 * 60 * 1000)); // UTC-3
        return $filter('date')(brasiliaDate, format || 'dd/MM/yyyy HH:mm');
    };
})

// Controller principal para funcionalidades compartilhadas
.controller('MainController', ['$location', function($location) {
    var vm = this;

    // Verifica se a rota atual corresponde à viewLocation
    vm.isActive = function(viewLocation) {
        return viewLocation === $location.path();
    };

    // Inicialização do controller
    vm.init = function() {
    };

    vm.init();
}]);
