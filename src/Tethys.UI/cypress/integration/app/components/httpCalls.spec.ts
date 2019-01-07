/// <reference types="cypress"/>

import { commander, verifier, mockServer } from "../../../testSdk";
import { httpCallListMap } from "../../../pageMaps/httpCallListMap";

const httpCallUrl = "httpCall";

beforeEach(() => {
    mockServer.mockHttpcalls([{
        method: 'GET',
        url: '**/httpCalls',
        response: 'fixture:httpCallData.json',
    }]);
    commander.goToUrl(httpCallUrl);
});

describe('httpCalls - check http-calls list', () => {
    it("Loads datatable as expected", () => {

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

    it.only("Click on line load content for preview", () => {
        commander.click({ text: { contains: "Details" } });
        commander.isVisible(httpCallListMap.httpCallDetails.collapse);
    });

    it("Click on Details moves to edit screen", () => {
        commander.click({ text: { contains: "Edit" } });
        cy.url().should('contain', '/httpcall/1/edit');
    });

});
