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

const formularioEntrada = await fs.readFile(
    new URL('../VisoERP.Web/Views/Entradas/Formulario.cshtml', import.meta.url), 'utf8');
assert.match(formularioEntrada,
    /data-campo="CustoUnitario" readonly aria-readonly="true"/);
assert.match(formularioEntrada,
    /custo\.value = window\.VisoMoney\.formatarValorServidor\(e\.target\.selectedOptions\[0\]\?\.dataset\.custo \|\| 0\);/);
assert.doesNotMatch(formularioEntrada, /VencimentoPagamento|CodigoLote|Validade/);

const modeloEntrada = await fs.readFile(
    new URL('../VisoERP.Web/Models/Estoque/EntradaEstoqueViewModel.cs', import.meta.url), 'utf8');
assert.doesNotMatch(modeloEntrada, /VencimentoPagamento|CodigoLote|Validade/);

const formularioVenda = await fs.readFile(
    new URL('../VisoERP.Web/Views/Vendas/Formulario.cshtml', import.meta.url), 'utf8');
assert.match(formularioVenda,
    /data-campo="PrecoCatalogo" readonly aria-readonly="true"/);
assert.match(formularioVenda,
    /data-campo="PrecoUnitario" readonly aria-readonly="true"/);
assert.match(formularioVenda,
    /precoVenda\.value = window\.VisoMoney\.formatarValorServidor\(option\?\.dataset\.precoVenda \|\| 0\);/);

const formularioSuprimento = await fs.readFile(
    new URL('../VisoERP.Web/Views/Suprimentos/Formulario.cshtml', import.meta.url), 'utf8');
assert.doesNotMatch(formularioSuprimento, /asp-for="CodigoInterno"/);

for (const caminho of [
    '../VisoERP.Web/Views/Clientes/Formulario.cshtml',
    '../VisoERP.Web/Views/Fornecedores/Formulario.cshtml']) {
    const formularioCadastro = await fs.readFile(new URL(caminho, import.meta.url), 'utf8');
    assert.doesNotMatch(formularioCadastro, /asp-for="Documento"|asp-for="Email"/);
}

console.log('Máscara monetária por centavos validada.');
