import { utils } from "./utils";

export interface DomElementDescriptor {
    css?: string;
    text?: {
        contains?: string;
    };
}

const domSynchronizer = {
    getByDescriptor: (descriptor: DomElementDescriptor) => {
        if (descriptor.css && utils.stringHasValue(descriptor.css)) {
            return cy.get(descriptor.css);
        }

        if (descriptor.text) {
            if (descriptor.text.contains && utils.stringHasValue(descriptor.text.contains)) {
                return cy.contains(descriptor.text.contains);
            }
            // other textual conditional goes here
        }
        throw new RangeError(`cannot locate dom element for '${JSON.stringify(descriptor)}'`);
    }
};

export const mockServer = {
    mockHttpcalls: (httpCalls: Array<{ method: string; url: string; response: any; }>) => {
        cy.server();
        for (const hc of httpCalls) {
            cy.route(hc);
        }
    }
};
export const commander = {
    goToUrl: (url: string) => {
        cy.visit(url);
    },
    click: (descriptor: DomElementDescriptor) => {
        domSynchronizer.getByDescriptor(descriptor).click();
    }
};

export const verifier = {
    haveLength: (descriptor: DomElementDescriptor, expectedLength: number) => {
        shoulder(domSynchronizer.getByDescriptor(descriptor), "be.have.length", expectedLength);
    },

    equals: (descriptor: DomElementDescriptor, expectedValue: string) => {
        shoulder(domSynchronizer.getByDescriptor(descriptor), ($ded: any) => expect($ded.text().trim()).to.eq(expectedValue));
    }
};

const shoulder = (predicate: Cypress.Chainable, chainer: any, expectedValue?: any) => {
    predicate.should(chainer, expectedValue);
};
