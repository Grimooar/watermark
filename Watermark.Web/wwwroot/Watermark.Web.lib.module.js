export function beforeStart(options, extensions) {
    var customScript = document.createElement('script');
    customScript.setAttribute('src', 'js/Preview.js');
    document.head.appendChild(customScript);
}