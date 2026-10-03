# Vendas em Rascunho — Especificação

## Objetivo

Permitir que uma nova venda seja salva como rascunho, editada e excluída sem produzir efeitos reais no estoque, financeiro ou indicadores. Somente a ação explícita de finalizar transforma o rascunho em venda real.

## Regras de negócio

- O cliente é obrigatório para salvar um rascunho.
- Itens, desconto, entrada, quantidade de parcelas e vencimento podem ser alterados enquanto a venda estiver em rascunho.
- Rascunho não baixa estoque, não cria movimentação de saída, não registra recebimento e não entra nos indicadores financeiros.
- Rascunho pode ser reaberto e editado.
- Rascunho pode ser excluído integralmente, incluindo seus itens e parcelas eventualmente salvos.
- Apenas rascunhos exibem as ações Editar e Excluir.
- Finalizar uma venda valida itens, desconto, entrada, parcelas, vencimento e estoque; em uma única transação, efetiva a saída de estoque, cria parcelas/recebimentos aplicáveis e marca `Finalizada`.
- Venda finalizada não pode voltar a rascunho nem usar o fluxo de edição de rascunho.

## Fluxo de usuário

### Nova venda

O usuário acessa Nova venda, informa o cliente e pode salvar o formulário como rascunho. O botão Voltar/Salvar rascunho persiste as alterações e retorna à listagem. Se o cliente não estiver informado, o formulário permanece aberto com erro de validação.

### Edição

Na listagem, rascunhos aparecem identificados com o status Rascunho e possuem Editar. A edição reutiliza o formulário de venda e permite salvar novamente ou finalizar.

### Finalização

O botão Finalizar venda executa a operação de negócio completa. Falhas de validação ou estoque não alteram o rascunho nem o estoque. Em caso de sucesso, o usuário é direcionado aos detalhes da venda finalizada.

### Exclusão

Excluir rascunho exige confirmação e remove o rascunho e seus dados dependentes. A ação não aparece para vendas finalizadas.

## Arquitetura

O domínio continuará usando `Finalizada` como estado. O app service será separado em operações de salvar rascunho, atualizar rascunho, finalizar e excluir rascunho. A criação/atualização do rascunho apenas persiste a entidade e seus itens; a lógica atualmente executada em `RegistrarAsync` para estoque, recebimentos e parcelas será deslocada para a finalização.

O repositório deverá consultar rascunhos com itens e parcelas e permitir a exclusão controlada. O controller terá endpoints distintos para salvar, editar, finalizar e excluir, protegendo operações de edição para registros ainda não finalizados.

## Dados e compatibilidade

Não será criada uma nova coluna: `Vendas.Finalizada` já representa o estado necessário. Dados existentes com `Finalizada = true` permanecem vendas reais. A consulta de indicadores deverá filtrar ou receber apenas vendas finalizadas quando necessário.

## Testes e critérios de aceite

- Salvar rascunho sem cliente falha com validação.
- Salvar rascunho com cliente persiste venda e itens sem movimentação de estoque e sem recebimento.
- Editar rascunho substitui seus itens e valores.
- Excluir rascunho remove a venda e dependências.
- Finalizar rascunho baixa estoque, gera parcelas/entrada e marca a venda como finalizada.
- Falha na finalização mantém o rascunho e não baixa estoque.
- Venda finalizada não pode ser editada ou excluída pelo fluxo de rascunho.
- A listagem apresenta a ação Editar somente para rascunhos.
