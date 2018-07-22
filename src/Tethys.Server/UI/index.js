var link = 'http://localhost:4880/tethys/api/log';
// bootstrap the demo
new Vue({
    el: '#app',
    data: {
        searchQuery: '',
        gridColumns: ['httpMethod', 'uri', 'body', 'headers'],
        gridData: []
    },
    filters: {
        uppercase: function (v) {
            return v.toUpperCase();
        }
    }, methods: {
        getHttpCalls: function () {
            this.$http.get(link).then(function (response) {
                console.log(response.data);
                this.gridData = response.data.map(r => r.request);
                console.log(this.gridData);
            });
        }
    },
    beforeMount() {
        this.getHttpCalls()
    },
})
