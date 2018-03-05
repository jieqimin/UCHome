Array.prototype.Select = function (selector) {
	return Enumerable.From(this).Select(selector);
};

Array.prototype.SelectMany = function (collectionSelector, resultSelector) {
	return Enumerable.From(this).SelectMany(collectionSelector, resultSelector);
};

Array.prototype.ForEach = function (action) {
	return Enumerable.From(this).ForEach(action);
};

Array.prototype.Where = function (predicate) {
	return Enumerable.From(this).Where(predicate);
};

Array.prototype.OrderBy = function (keySelector) {
	return  Enumerable.From(this).OrderBy(keySelector);
};

Array.prototype.OrderByDescending = function (keySelector) {
	return Enumerable.From(this).OrderByDescending(keySelector);
};

Array.prototype.AsEnumerable = function () {
	return Enumerable.From(this);
};

Array.prototype.Sum = function () {
    return Enumerable.From(this).Sum();
};
