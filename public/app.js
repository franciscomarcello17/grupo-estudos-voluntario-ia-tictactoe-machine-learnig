angular.module('jogoDaVelhaApp', [
    'ngRoute'
]).controller('MainController', ['$location', function($location) {
    var vm = this;
    
    vm.isActive = function(viewLocation) {
        return viewLocation === $location.path();
    };
}]);