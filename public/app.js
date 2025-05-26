// Configuração do módulo principal
angular.module('jogoDaVelhaApp', [
    'ngRoute'
])

// Configuração para usar HTML5 Mode (URL sem hashbang)
.config(['$locationProvider', '$sceProvider', function($locationProvider, $sceProvider) {
    $locationProvider.html5Mode(true);
    $sceProvider.enabled(false);
}])

// Filtro personalizado para ajustar a data para o fuso de Brasília
.filter('brasiliaDate', function($filter) {
    return function(utcDateString, format) {
        if (!utcDateString) return '';
        const utcDate = new Date(utcDateString);
        const brasiliaDate = new Date(utcDate.getTime() - (3 * 60 * 60 * 1000)); // UTC-3
        return $filter('date')(brasiliaDate, format || 'dd/MM/yyyy HH:mm');
    };
})

.controller('MainController', ['$location', '$document', '$scope', function($location, $document, $scope) {
    var vm = this;

    vm.menuAberto = false;

    // Alterna o estado do menu ou força fechar
    vm.toggleMenu = function(forceClose) {
        if (typeof forceClose === 'boolean') {
            vm.menuAberto = forceClose;
        } else {
            vm.menuAberto = !vm.menuAberto;
        }
    };

    // Fecha o menu ao navegar (quando um item é clicado)
    vm.navigate = function() {
        vm.toggleMenu(false); // Fecha o menu
        // A navegação em si já será tratada pelo ng-href/ng-click
    };

    // Verifica se a rota atual corresponde à viewLocation
    vm.isActive = function(viewLocation) {
        return viewLocation === $location.path();
    };

    // Fecha o menu se clicar fora
    function handleClickOutside(event) {
        var menuToggle = angular.element(document.querySelector('.menu-toggle'))[0];
        var mainNav = angular.element(document.querySelector('.main-nav'))[0];
        
        if (!mainNav.contains(event.target) && !menuToggle.contains(event.target)) {
            $scope.$apply(function() {
                vm.toggleMenu(false);
            });
        }
    }

    vm.init = function() {
        $document.on('click', handleClickOutside);
    };

    $scope.$on('$destroy', function() {
        $document.off('click', handleClickOutside);
    });

    vm.init();
}]);