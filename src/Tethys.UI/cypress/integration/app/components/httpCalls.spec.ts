/// <reference types="cypress"/>

import { commander, verifier, mockServer } from "../../../testSdk";
import { httpCallListMap } from "../../../pageMaps/httpCallListMap";

const httpCallUrl = "httpCall";

describe('httpCalls - check http-calls list', () => {
    it("Loads datatable as expected", () => {
        mockServer.mockHttpcalls([{
            method: 'GET',
            url: '**/httpCalls',
            response: 'fixture:httpCallData.json'
        }]);

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
