export const httpCallListMap = {
    dataTable: {
        header: {
            css: "th",
            id: { css: 'th:nth-child(1)' },
            httpMethod: { css: 'th:nth-child(2)' },
            usage: { css: 'th:nth-child(3)' },
            name: { css: 'th:nth-child(4)' },
            commands: { css: 'th:nth-child(5)' },
        },
        rows: {
            css: "tbody tr"
        },
        pagination: {
            css: "#pagination",
            label: { css: '#pagination [class^="label"]' },
            next: { css: '#pagination [class^="next"]' }
        }
    },
    httpCallDetails: {
        collapse: {
            css: ".collapse"
        }
    }
};
