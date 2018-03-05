function convertimg(path, type) {
    var newpath = "";
    if (path.length > 0) {
        var ext = path.substr(path.lastIndexOf('.'));
        var newext;
        switch (type) {
            case "thumbnail":
                newext = "_thumbnail" + ext;
                break;
            case "mini":
                newext = "_mini" + ext;
                break;
            case "headimg":
                newext = "_headimg" + ext;
                break;
            default:
                newext = ext;
                break;
        }
        newpath = path.replace(ext, newext);
    }
    return newpath;
}