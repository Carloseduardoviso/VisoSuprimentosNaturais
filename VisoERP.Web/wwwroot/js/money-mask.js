(() => {
    const inteiroAgrupado = /^\d{1,3}(?:\.\d{3})+$/;

    function separar(valor, escala = 2) {
        const texto = String(valor ?? '').replace(/[^\d.,+-]/g, '');
        if (!texto || !/\d/.test(texto)) return null;
        const virgula = texto.lastIndexOf(',');
        let separadorDecimal = virgula;
        if (virgula < 0) {
            const ponto = texto.lastIndexOf('.');
            if (ponto >= 0 && !inteiroAgrupado.test(texto)) separadorDecimal = ponto;
        }
        const brutoInteiro = separadorDecimal >= 0 ? texto.slice(0, separadorDecimal) : texto;
        const brutoDecimal = separadorDecimal >= 0 ? texto.slice(separadorDecimal + 1) : '';
        const inteiro = brutoInteiro.replace(/\D/g, '').replace(/^0+(?=\d)/, '') || '0';
        const decimal = brutoDecimal.replace(/\D/g, '').slice(0, escala);
        return { inteiro, decimal, temDecimal: separadorDecimal >= 0, escala };
    }

    function formatar(valor, completo = false, escala = 2) {
        const partes = separar(valor, escala);
        if (!partes) return '';
        partes.inteiro = partes.inteiro.replace(/\B(?=(\d{3})+(?!\d))/g, '.');
        const decimal = completo ? partes.decimal.padEnd(escala, '0') : partes.decimal;
        return partes.inteiro + (partes.temDecimal || completo ? ',' + decimal : '');
    }

    function numero(valor, escala = 2) {
        const partes = separar(valor, escala);
        return partes ? Number(partes.inteiro + '.' + partes.decimal) : 0;
    }

    function atualizar(campo, completo = false) {
        if (!campo?.matches('[data-money-mask]')) return;
        const anterior = campo.value;
        const inicio = campo.selectionStart;
        const prefixo = inicio === null ? anterior : anterior.slice(0, inicio);
        const digitosAntes = (prefixo.match(/\d/g) || []).length;
        const decimalAntes = prefixo.includes(',') ||
            (prefixo.includes('.') && !inteiroAgrupado.test(prefixo));
        const escala = Number(campo.dataset.moneyScale || 2);
        const formatado = formatar(anterior, completo, escala);
        if (anterior === formatado) return;
        campo.value = formatado;

        if (inicio !== null && document.activeElement === campo) {
            let posicao = 0;
            let vistos = 0;
            while (posicao < formatado.length && vistos < digitosAntes) {
                if (/\d/.test(formatado[posicao])) vistos++;
                posicao++;
            }
            const virgula = formatado.indexOf(',');
            if (decimalAntes && virgula >= 0) posicao = Math.max(posicao, virgula + 1);
            campo.setSelectionRange(posicao, posicao);
        }
    }

    document.addEventListener('input', evento => atualizar(evento.target));
    document.addEventListener('focusout', evento => atualizar(evento.target, true));
    document.addEventListener('DOMContentLoaded', () =>
        document.querySelectorAll('[data-money-mask]').forEach(campo => atualizar(campo, true)));
    document.addEventListener('submit', evento =>
        evento.target.querySelectorAll('[data-money-mask]').forEach(campo => atualizar(campo, true)), true);

    window.VisoMoney = { formatar, numero, atualizar };
})();
