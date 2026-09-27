(() => {
    if (!window.jQuery?.validator || !window.VisoMoney) return;

    const validator = window.jQuery.validator;
    const validarNumeroOriginal = validator.methods.number;
    const validarFaixaOriginal = validator.methods.range;
    const formatoMonetario = /^\d{1,3}(?:\.\d{3})*,\d{2}$/;

    validator.methods.number = function (value, element) {
        if (!element.matches('[data-money-mask]'))
            return validarNumeroOriginal.call(this, value, element);
        return this.optional(element) || formatoMonetario.test(value);
    };

    validator.methods.range = function (value, element, parametros) {
        if (!element.matches('[data-money-mask]'))
            return validarFaixaOriginal.call(this, value, element, parametros);
        const numero = window.VisoMoney.numero(value);
        return this.optional(element) || (numero >= parametros[0] && numero <= parametros[1]);
    };
})();
