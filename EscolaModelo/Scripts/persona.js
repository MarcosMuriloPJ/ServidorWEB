// URL da WEB API
var rootURL = "http://localhost:60877/"

/******************
        Funções
******************/
// Lista todos os alunos cadastrados
function Consultar() {
    // Mostra a div correspondente às mensagens de retorno das ações
    $('#msg-retorno').show()

    $.ajax({
        url: rootURL + 'Aluno/Consultar/',
        type: 'GET',
        success: function (data) {
            // Limpa as linhas da tabela de alunos
            $('#lista-alunos').html('')

            // Verifica se tem aluno cadastrado
            if (data.length > 0) {
                var html = ''

                // Loop que monta o HTML das linhas a serem inseridas na tabela
                for (var i = 0; i < data.length; i++) {
                    html += ' \
                        <tr data-id="' + data[i].Id + '" data-nome="' + data[i].Nome + '" data-toggle="modal" data-target="#modalDel"> \
                            <td>' + data[i].CPF + '</td> \
                            <td>' + data[i].Nome + '</td> \
                            <td>' + data[i].DataNasc + '</td> \
                        </tr>'
                }

                $('#lista-alunos').html(html)

                // Reatribui a ação 'click' às linhas da tabela
                $('#lista-alunos tr').on('click', (function () {
                    var nome = $(this).attr("data-nome")
                    var id = $(this).attr("data-id")
                    $('#nome-aluno').html(nome)
                    $('#HdnIdAluno').val(id)
                }))
            }
        },
        error: function (xhr, status, error) {
            // Seta a cor do alerta
            $('#msg-retorno').addClass('alert-danger')
            // Seta a mensagem do alerta
            $('#msg-retorno').html('<strong>Desculpe!</strong> Não foi possível apagar o Aluno!')
        }
    })
}

function Apagar(id) {
    // Mostra a div correspondente às mensagens de retorno das ações
    $('#msg-retorno').show()

    $.ajax({
        url: rootURL + 'Aluno/Delete/' + id,
        type: 'GET',
        success: function (data) {
            if (data == true) {
                Consultar()
                // Seta a cor do alerta
                $('#msg-retorno').addClass('alert-success')
                // Seta a mensagem do alerta
                $('#msg-retorno').html('<strong>Sucesso!</strong> O aluno foi excluído!')
            }
            else {
                // Seta a cor do alerta
                $('#msg-retorno').addClass('alert-danger')
                // Seta a mensagem do alerta
                $('#msg-retorno').html('<strong>Desculpe!</strong> Não foi encontrado este aluno!')
            }
        },
        error: function (xhr, status, error) {
            // Seta a cor do alerta
            $('#msg-retorno').addClass('alert-danger')
            // Seta a mensagem do alerta
            $('#msg-retorno').html('<strong>Desculpe!</strong> Não foi possível apagar o Aluno!')
        }
    })
}
/******************
        Funções
******************/

// Título inicial
$("#titulo-principal").html("Importação")

// Troca do título inicial ao clicar na aba 'Visualização'
$('#tab-visualizacao').click(function () {
    $("#titulo-principal").html("Visualização")
})

// Troca do título inicial ao clicar na aba 'Importação'
$('#tab-importacao').click(function () {
    $("#titulo-principal").html("Importação")
})

// Pega os valores do aluno clicado na tabela (para exclusão)
$('#lista-alunos tr').on('click', (function () {
    var nome = $(this).attr("data-nome")
    var id = $(this).attr("data-id")
    $('#nome-aluno').html(nome)
    $('#HdnIdAluno').val(id)
}))

// Pega o id e envia para exclusão do aluno
$('#del-aluno').unbind('click').click(function () {
    var id = $('#HdnIdAluno').val()
    Apagar(id)
})