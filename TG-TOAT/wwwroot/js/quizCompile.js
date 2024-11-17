
function CompileAnswers() {
    var radio = 0
    var questCards = document.getElementsByClassName("questCard");

    var submission = "";

    for (let i = 0; i < questCards.length; i++) {
        if (questCards[i].innerHTML.includes("radio")) {
            radio = 1
            submission += ("2д");
            var question = questCards[i].getElementsByTagName("h5");
            submission += (question[0].textContent);
            var mcChoices = questCards[i].getElementsByClassName("mcChoice")
            submission += "ж";
            for (let j = 0; j < mcChoices.length; j++) {
                submission += ("ч");
                if (mcChoices[j].checked) {
                    submission += ("г");
                    var selector = 'label[for=' + mcChoices[j].id + ']';
                    var label = document.querySelector(selector);
                    submission += (label.textContent)
                }
                else {
                    var selector = 'label[for=' + mcChoices[j].id + ']';
                    var label = document.querySelector(selector);
                    submission += (label.textContent)
                }

            }

        }
        else {
            radio = 0
            submission += ("1д");
            var question = questCards[i].getElementsByTagName("h5");
            submission += (question[0].textContent);
            var text = questCards[i].getElementsByClassName("text-entry");
            submission += ("ж");
            submission += (text[0].value);
        }
        submission += ('п');
        var points = questCards[i].getElementsByClassName("questPoint");
        var pointsNum = points[0].textContent.split(" ")
        if (radio == 1) {
            submission += "0/";
        }
        else {
            submission += "NG/";
        }
        
        submission += pointsNum[0];
        submission +=("з");
    }
    var sub = document.getElementById("quizSub")

    sub.value = submission;

    //console.log(submission);
    
}