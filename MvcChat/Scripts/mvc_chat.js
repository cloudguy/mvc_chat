Date.prototype.chatFormat = function () {
    var year = this.getFullYear(); var month = this.getMonth();
    var day = this.getDate(); var hours = this.getHours(); var mins = this.getMinutes(); var secs = this.getSeconds();
    return day + '/' + month + '/' + year + ' ' + hours + ':' + mins + ':' + secs;
}

function Chat() {
    var chat = {};

    chat.LastCheckedMessageId = 0;
    chat.LoadInitialDataUrl = '';
    chat.CheckNewMessagesUrl = '';
    
    var emoticons = {
        threaten: { img: '<img src="Content/Smilies/threaten.gif" />', symbol: '>:-(', encoded: /&gt;:-\(/gi },
        sorry: { img: '<img src="Content/Smilies/sorry.gif" />', symbol: ':-<', encoded: /:-&lt;/gi },
        cry: { img: '<img src="Content/Smilies/cry.gif" />', symbol: '&-<', encoded: /&amp;-&lt;/gi },
        agree: { img: '<img src="Content/Smilies/agree.gif" />', symbol: '>:-|', encoded: /&gt;:-\|/gi },
        smile: { img: '<img src="Content/Smilies/smile.gif" />', symbol: ':-)', encoded: /:-\)/gi },
        sad: { img: '<img src="Content/Smilies/sad.gif" />', symbol: ':-(', encoded: /:-\(/gi }
    };
  
    chat.ReplaceEmoticons = function (msgHtml) {
        $.each(emoticons, function (index, value) {
            msgHtml = msgHtml.replace(value.encoded, value.img);
        });
        return msgHtml;
    }



    chat.LoadInitialData = function () {       
        $.ajax({
            url: chat.LoadInitialDataUrl,
            type: 'POST',
            success: function (data) {
                if (data.Messages)
                    chat.ShowMessages(data.Messages);
            }
        });
    }

    chat.InsertText = function (el, text) {        
        var val = el.value, endIndex, range;
        if (typeof el.selectionStart != "undefined" && typeof el.selectionEnd != "undefined") {
            endIndex = el.selectionEnd;
            el.value = val.slice(0, endIndex) + text + val.slice(endIndex);
            el.selectionStart = el.selectionEnd = endIndex + text.length;
        } else if (typeof document.selection != "undefined" && typeof document.selection.createRange != "undefined") { //stupid IE
            el.focus();
            range = document.selection.createRange();
            range.collapse(false);
            range.text = text;
            range.select();
        }
        return false;
    }

    chat.Init = function (messageCheckTimeout, initialDataUrl, checkMessagesUrl) {
        chat.LoadInitialDataUrl = initialDataUrl;
        chat.CheckNewMessagesUrl = checkMessagesUrl;
        chat.MessageCheckTimeout = messageCheckTimeout||3000;
        $.ajaxSetup({
            error: function () {
                chat.ShowErrors("Server error");
            }
        });

        var emoButtons = $('#emo_buttons');
        $.each(emoticons, function (index, value) {
            var button = $('<a href="#" class="btn" style="width: 30px; height: 20px;"></a>').append(value.img);
            button.click(function () {
                var el = document.getElementById("Message");
                chat.InsertText(el, value.symbol);
                return false;
            });
            emoButtons.append(button);
        });


        $('#new_msg_form').submit(function () {
            if (!$(this).valid())
                return false;

            var isSuccess = false;
            var url = $(this).attr('action');
            $(this).find('#LastCheckedId').val(chat.LastCheckedMessageId);
            var data = $(this).serialize();
            $.ajax({
                url: url,
                type: 'POST',
                data: data,
                success: function (data) {
                    if (data.Errors)
                        chat.ShowErrors(data.Errors);
                    else
                        isSuccess = true;
                    if (data.Messages)
                        chat.ShowMessages(data.Messages);
                },
                complete: function () {
                    if (isSuccess) {
                        $('#Message').val('');
                        $('#chat_errors').hide();
                    }
                }
            });
            return false;
        });

        chat.LoadInitialData();
        setInterval(chat.CheckNewMessages, chat.MessageCheckTimeout);
        return chat;
    }

    chat.CheckNewMessages = function () {
        $.ajax({
            url: chat.CheckNewMessagesUrl,
            type: 'POST',
            data: { lastCheckedId: chat.LastCheckedMessageId },
            success: function (data) {
                if (data.Messages)
                    chat.ShowMessages(data.Messages);
            }
        });
    }    

    chat.ShowMessages = function (messages) {
        var historyControl = $('#msg_history');
        var hasNew = false;
        //jquery templates could be used here, but for simple markup this works fine
        $.each(messages, function (index, msg) {
            var dateTime = new Date(msg.SendTime * 1000); //from unixtime
            var userName = $('<div class="chat_nickname"></div>').text(msg.UserName + ' - ' + dateTime.chatFormat());
            var messageText = $('<div></div>').text(msg.Text);
            var msgHtml = messageText.html();
            messageText.html(chat.ReplaceEmoticons(msgHtml));
            var fullMessageEntry = $('<div></div>').append(userName).append(messageText).appendTo(historyControl);
            if (chat.LastCheckedMessageId < msg.Id)
                chat.LastCheckedMessageId = msg.Id;
            hasNew = true;
        });
        if (hasNew)
            $('#msg_history_scroll').scrollTop(historyControl.height());
    }

    chat.ShowErrors = function (errors) {
        var errors_container = $('#chat_errors');
        var errlist = $('<ul></ul>');
        if (typeof errors == 'string') {
            $('<li></li>').text(errors).appendTo(errlist);
        }
        else {
            $.each(errors, function (index, errvalue) {
                $('<li></li>').text(errvalue).appendTo(errlist);
            });
        }        
        errors_container.empty();
        errors_container.append(errlist);
        errors_container.show();
    }

    return chat;
}