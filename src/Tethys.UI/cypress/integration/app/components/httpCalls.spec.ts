/// <reference types="cypress"/>

const domSynchronizer = {
    getByDescriptor: (descriptor: DomElementDescriptor) => {
        if (stringUtil.hasValue(descriptor.css)) {
            return cy.get(descriptor.css);
        }

        if (stringUtil.hasValue(descriptor.text.contains)) {
            return cy.contains(descriptor.text.contains);
        }
    }
};

const commander = {
    goToUrl: (url: string) => {
        cy.visit(url);
    },
    click: (descriptor: DomElementDescriptor) => {
        domSynchronizer.getByDescriptor(descriptor).click();
    }
};

const stringUtil = {
    hasValue: (source: string): boolean => {
        return source && source.trim().length > 0;
    }
};

const verifier = {
    haveLength: (descriptor: DomElementDescriptor, expectedLength: number) => {
        shoulder(domSynchronizer.getByDescriptor(descriptor), "be.have.length", expectedLength);
    },

    equals: (descriptor: DomElementDescriptor, expectedValue: string) => {
        shoulder(domSynchronizer.getByDescriptor(descriptor), ($ded) => expect($ded.text().trim()).to.eq(expectedValue));
    }
};

const shoulder = (predicate: Cypress.Chainable, chainer: any, expectedValue?: any) => {
    predicate.should(chainer, expectedValue);
};

export interface DomElementDescriptor {
    css?: string;
    text?: {
        contains?: string;
    };
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
            css: "tbody tr"
        },
        pagination: {
            css: "#pagination",
            label: { css: '#pagination [class^="label"]' },
            next: { css: '#pagination [class^="next"]' }
        }
    }
};

const httpCallUrl = "http://localhost:3000/httpCall";

describe('httpCalls - check http-calls list', () => {
    it("Loads datatable as expected", () => {
        cy.server();
        cy.route({
            method: 'GET',
            url: '**/httpCalls',
            response: 'fixture:httpCallData.json'
        });

        commander.goToUrl(httpCallUrl);

        // test headers
        verifier.haveLength(httpCallListMap.dataTable.header, 5);

        verifier.equals(httpCallListMap.dataTable.header.id, "Id");
        verifier.equals(httpCallListMap.dataTable.header.httpMethod, "Http Method");
        verifier.equals(httpCallListMap.dataTable.header.usage, "Usage (%)");
        verifier.equals(httpCallListMap.dataTable.header.name, "Name");
        verifier.equals(httpCallListMap.dataTable.header.commands, "Commands");

        // test pagination
        verifier.haveLength(httpCallListMap.dataTable.rows, 11);
        verifier.equals(httpCallListMap.dataTable.pagination.label, "1 / 5");
        // go to last page
        for (let i = 0; i < 4; i++) {
            commander.click(httpCallListMap.dataTable.pagination.next);
        }
        verifier.haveLength(httpCallListMap.dataTable.rows, 4);
        verifier.equals(httpCallListMap.dataTable.pagination.label, "5 / 5");
    });

    it("Click on line load content for preview", () => {
        throw new Error("Not Implemented");
    });

    it("Click on Details moves to edit screen", () => {
        cy.server();
        cy.route({
            method: 'GET',
            url: '**/httpCalls',
            response: 'fixture:httpCallData.json'
        });

        commander.goToUrl(httpCallUrl);
        commander.click({ text: { contains: "Edit" } });
        cy.url().should('contain', '/httpcall/1/edit');
    });

});
