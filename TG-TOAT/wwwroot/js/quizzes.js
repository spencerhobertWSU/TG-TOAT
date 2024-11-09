
function updateNumQuest() {
    var quest = document.getElementById("numQuestions");
    var questions = document.getElementById("Questions")

    let newQuestions = ""

    let totalq = Number(quest.value)

    for (let i = 1; i < totalq + 1; i++) {
        newQuestions += ('<div>')
        newQuestions += ('<div class="input-group questionInfo">')

        newQuestions += ('<select class="form-select" id="select' + i + '" onchange="updateAnswer(' + i +')" required>');
        newQuestions += ('<option selected>Choose...</option>');
        newQuestions += ('<option value="1">Text Entry</option>');
        //newQuestions += ('<option value="2">File Submission</option>');
        newQuestions += ('<option value="2">Multiple Choice</option>');
        newQuestions += ('</select>');


        newQuestions += ('<div class="form-floating" id="qText">')
        newQuestions += ('<input for="q' + i + ' type=" Text" class="form-control" id="q' + i + '" name="q' + i + '" placeholder="Question ' + i + '"/ required>')
        newQuestions += ('<label for="q' + i + '">Question ' + i + '</label>')
        newQuestions += ('</div>')

        newQuestions += ('<div class="form-floating" id = "qPoints" >')
        newQuestions += ('<input for="p' + i + '" type="number" class="form-control" id="p' + i + '" name="p' + i + '" min="0" placeholder="Points" onchange="updatePoints()" / required>')
        newQuestions += ('<label for="p' + i + '">Points</label>')
        newQuestions += ('</div >')

        newQuestions += ('</div>')

        newQuestions += ('<div id = "a' + i + '" >')

        newQuestions += ('</div>')
        newQuestions += ('</div >')
        newQuestions += ('</div >')

    };

    questions.innerHTML = newQuestions


}
function updateAnswer(num) {
    
    var choice = document.getElementById(("select" + num))
    var answer = document.getElementById(("a"+num))

    let answerHtml = ""

    if (choice.value == 2) {
        answerHtml += '<div class="form-floating" id="mcNum">'
        answerHtml += '<input for="mcNum" type="number" class="form-control" id="mcNum' + num + '" name="mcNum" min="2" max="5" placeholder="Number of Choices" onchange="updatemc(' + num + ')" required/>'
        answerHtml += '<label for="mcNum">Number of Choices</label>'
        answerHtml += '</div>'
        answerHtml += '<div id="mc' + num + '">'
        answerHtml += '</div>'

        answer.innerHTML = answerHtml
    }
    else {
        answer.innerHTML = ""
    }
}

function updatemc(mcNum) {
    var numChoices = document.getElementById(("mcNum" + mcNum))
    var choices = document.getElementById(("mc" + mcNum))

    let newChoices = ""
    let totalc = Number(numChoices.value)

    for (let i = 1; i < totalc + 1; i++) {
        newChoices += ('<div class="form-check">')
        newChoices += ('<input class="form-check-input" type="radio" name="q' + mcNum + 'c" id="q' + mcNum + 'r' + i +'" required>')
        newChoices += ('<input for="q' + mcNum + 'c" type="Text" class="form-control" id="q' + mcNum + 't' + i + '" name="q' + mcNum + 'c" placeholder="Choice' + i +'" required/>')
        newChoices += ('</div>')
    };

    choices.innerHTML = newChoices
}

function updatePoints() {
    var overallPoints = document.getElementById("QuizPoints")
    var quest = document.getElementById("numQuestions");
    var points = 0


    let totalq = Number(quest.value)


    for (let i = 1; i < totalq + 1; i++) {
        var newPoints = document.getElementById('p' + i).value;
        newPoints = Number(newPoints)

        points += newPoints;
    }
    overallPoints.value = "";

    overallPoints.value = points;
}


function submitQuiz() {
    var overallPoints = document.getElementById("QuizPoints")
    var quest = document.getElementById("numQuestions");
    var points = 0

    let totalq = Number(quest.value)

    var allquestions = ""

    for (let i = 1; i < totalq + 1; i++) {
        var select = document.getElementById("select" + i)
        var question = document.getElementById("q" + i)
        var points = document.getElementById("p" + i)

        allquestions += select.value
        allquestions += "д"
        allquestions += question.value

        if (select.value == 2) {
            
            var numChoices = document.getElementById('mcNum' + i).value

            numChoices = Number(numChoices)

            for (let j = 1; j < numChoices+1; j++) {

                console.log('q' + i + 'r' + j)

                var radio = document.getElementById('q' + i+'r'+j)
                var text = document.getElementById('q' + i + 't' + j)
                allquestions += "ч"
                if (radio.checked) {
                    allquestions += "г"
                }
                allquestions += text.value
            }
            
        }

        allquestions += "п"
        allquestions += points.value

        allquestions += "з"
    }
    document.getElementById("hideQuest").value = allquestions
}

