import assert from 'node:assert/strict';
import fs from 'node:fs/promises';
import vm from 'node:vm';

const source = await fs.readFile(
    new URL('../VisoERP.Web/wwwroot/js/money-mask.js', import.meta.url), 'utf8');
const context = { document: { addEventListener() {} }, window: {} };
vm.runInNewContext(source, context);

assert.equal(context.window.VisoMoney.formatarPorCentavos('830000'), '8.300,00');
assert.equal(context.window.VisoMoney.formatarPorCentavos('868'), '8,68');
assert.equal(context.window.VisoMoney.formatarPorCentavos('0'), '0,00');
assert.equal(context.window.VisoMoney.numero('8.300,00'), 8300);
assert.equal(context.window.VisoMoney.formatarValorServidor('69.90'), '69,90');
assert.equal(context.window.VisoMoney.formatarValorServidor('8300.00'), '8.300,00');

console.log('Máscara monetária por centavos validada.');
