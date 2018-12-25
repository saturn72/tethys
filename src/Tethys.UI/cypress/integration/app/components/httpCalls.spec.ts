/// <reference types="cypress"/>

const commander = {
    goToUrl: (url: string) => {
        cy.visit(url);
    }
};

const stringUtil = {
    hasValue: (source: string): boolean => {
        return source && source.trim().length > 0;
    }
};

const verifier = {
    haveLength: (descriptor: DomElementDescriptor, expectedLength: number) => {
        if (stringUtil.hasValue(descriptor.css)) {
            shoulder(cy.get(descriptor.css), "be.have.length", expectedLength);
            return;
        }
    },

    equals: (descriptor: DomElementDescriptor, expectedValue: string) => {
        if (stringUtil.hasValue(descriptor.css)) {
            shoulder(cy.get(descriptor.css), ($ded) => expect($ded.text().trim()).to.eq(expectedValue));
            return;
        }
    }
};

const shoulder = (predicate: Cypress.Chainable, chainer: any, expectedValue?: any) => {
    predicate.should(chainer, expectedValue);
};

export interface DomElementDescriptor {
    css?: string;
}

const httpCallListMap = {
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
            css: "tr"
        }
    }
};

describe('httpCalls - check http-calls list', () => {
    it("test data table on http-call list", async () => {
        commander.goToUrl("http://localhost:3000/httpCalls");

        // test headers
        verifier.haveLength(httpCallListMap.dataTable.header, 5);

        verifier.equals(httpCallListMap.dataTable.header.id, "Id");
        verifier.equals(httpCallListMap.dataTable.header.httpMethod, "Http Method");
        verifier.equals(httpCallListMap.dataTable.header.usage, "Usage (%)");
        verifier.equals(httpCallListMap.dataTable.header.name, "Name");
        verifier.equals(httpCallListMap.dataTable.header.commands, "Commands");

        // test content
        verifier.haveLength(httpCallListMap.dataTable.rows, 10);

    });
});
