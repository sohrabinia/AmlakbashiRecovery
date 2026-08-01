var filterSubject = {
	filterData: {},
	listeners: [],
	addListener: function (listener) {
		this.listeners.push(listener);
    },
	updateData: function (newData , isBack) {
		this.listeners.forEach(function (listener) {
			listener(newData, isBack);
		});
	}
};