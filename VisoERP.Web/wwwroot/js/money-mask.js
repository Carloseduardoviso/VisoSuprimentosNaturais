(() => {
    function digitos(valor) {
        return String(valor ?? '').replace(/\D/g, '').replace(/^0+(?=\d)/, '');
    }

    function formatarPorCentavos(valor, escala = 2) {
        const bruto = digitos(valor);
        if (!bruto) return '';
        const preenchido = bruto.padStart(escala + 1, '0');
        const inteiro = preenchido.slice(0, -escala).replace(/^0+(?=\d)/, '') || '0';
        const decimal = preenchido.slice(-escala);
        return inteiro.replace(/\B(?=(\d{3})+(?!\d))/g, '.') + ',' + decimal;
    }

    function numero(valor, escala = 2) {
        const bruto = digitos(valor);
        return bruto ? Number(bruto) / (10 ** escala) : 0;
    }

    function atualizar(campo, completo = false) {
        if (!campo?.matches('[data-money-mask]')) return;
        const anterior = campo.value;
        const escala = Number(campo.dataset.moneyScale || 2);
        const formatado = formatarPorCentavos(anterior, escala);
        if (anterior === formatado) return;
        campo.value = formatado;
        campo.setSelectionRange(formatado.length, formatado.length);
    }

    document.addEventListener('input', evento => atualizar(evento.target));
    document.addEventListener('focusout', evento => atualizar(evento.target, true));
    document.addEventListener('DOMContentLoaded', () =>
        document.querySelectorAll('[data-money-mask]').forEach(campo => atualizar(campo, true)));
    document.addEventListener('submit', evento =>
        evento.target.querySelectorAll('[data-money-mask]').forEach(campo => atualizar(campo, true)), true);

    window.VisoMoney = { formatarPorCentavos, numero, atualizar };
})();
