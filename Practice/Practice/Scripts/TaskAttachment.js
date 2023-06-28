function handleFiles(input, preview, clearfilesid, savebtn = null) {
    input.addEventListener('change', updatePreview);
    $(clearfilesid).click(function clearFiles(event) {
        var $el = $('files');
        $el.wrap('form').closest('form').get(0).reset();
        $el.unwrap();
        updatePreview();
    });
    function updatePreview() {
        var invalidFilesSize = false;
        var invalidFilesName = false;
        while (preview.firstChild) {
            preview.removeChild(preview.firstChild);
        }
        var selectedFiles = document.getElementById('files').files;
        if (selectedFiles.length == 0) {
            var paragr = document.createElement('p');
            paragr.textContent = 'No files selected';
            preview.appendChild(paragr);
        } else {
            var totalsize = 0;
            var listRoot = document.createElement('ul');
            listRoot.style.listStyle = "none";
            listRoot.style.padding = "0";
            preview.appendChild(listRoot);
            for (var i = 0; i < selectedFiles.length; i++) {
                var li = document.createElement('li');
                var pr = document.createElement('p');
                totalsize = totalsize + selectedFiles[i].size;
                if (selectedFiles[i].name.length > 255 || selectedFiles[i].name.length == 0) {
                    invalidFilesName = true;
                }
                if (selectedFiles[i].size == 0) {
                    pr.textContent = selectedFiles[i].name + ' is empty';
                    pr.classList.add('text-danger');
                    invalidFilesSize = true;
                } else {
                    pr.textContent = selectedFiles[i].name + ', size: ' + getFileSize(selectedFiles[i].size);
                }
                li.appendChild(pr);
                listRoot.appendChild(li);
            }
        }
        if (invalidFilesSize || invalidFilesName) {
            if (savebtn != null) {
                savebtn.setAttribute('disabled', 'true');
            }
            var errorSize = document.createElement('p');
            var errorName = document.createElement('p');
            if (invalidFilesSize) {
                errorSize.textContent = 'Total upload size exceeds maximum size';
            }
            if (invalidFilesName) {
                errorName.textContent = 'Filename should be between 1 and 255 characters!';
            }
            errorSize.classList.add('text-danger');
            errorName.classList.add('text-danger');
            preview.appendChild(errorSize);
            preview.appendChild(errorName);
        } else {
            if (savebtn != null) {
                savebtn.removeAttribute('disabled');
            }
        }
    }
}

function getFileSize(number) {
    if (number < 1024) {
        return number + ' bytes';
    } else if (number > 1024 && number < 1048576) {
        return (number / 1024).toFixed(1) + ' KB';
    } else if (number > 1048576) {
        return (number / 1048576).toFixed(1) + ' MB';
    }
}