    var CopyPastePlugin =
    {
      CopyToClipboard: function(text) {
        const type = "text/plain";
        const blob = new Blob([UTF8ToString(text)], { type });
        const newText = UTF8ToString(text);
        console.log(newText);

        navigator.clipboard.writeText(newText).then(function() {
            console.log('Async: Copying to clipboard was successful!');
        }, function(err) {
            console.error('Async: Could not copy text: ', err);
            var textArea = document.createElement("textarea");

            // Place in the top-left corner of screen regardless of scroll position.
            textArea.style.position = 'fixed';
            textArea.style.top = 0;
            textArea.style.left = 0;

            // Ensure it has a small width and height. Setting to 1px / 1em
            // doesn't work as this gives a negative w/h on some browsers.
            textArea.style.width = '2em';
            textArea.style.height = '2em';

            // We don't need padding, reducing the size if it does flash render.
            textArea.style.padding = 0;

            // Clean up any borders.
            textArea.style.border = 'none';
            textArea.style.outline = 'none';
            textArea.style.boxShadow = 'none';

            // Avoid flash of the white box if rendered for any reason.
            textArea.style.background = 'transparent';

            console.log('text is');
            console.log(newText);
            textArea.value = newText;

            document.body.appendChild(textArea);
            textArea.focus();
            textArea.select();

            try {
                var successful = document.execCommand('copy');
                var msg = successful ? 'successful' : 'unsuccessful';
                console.log('Copying text command was ' + msg);
            } catch (err) {
                console.log('Oops, unable to copy');
            }

            document.body.removeChild(textArea);
        });
      }
    };
    mergeInto(LibraryManager.library, CopyPastePlugin);
