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

export interface HttpCall {
    method: string;
    url: string | RegExp;
    response?: any;
    delay?: number;
    status?: number;
    headers?: any;
    callbacks?: {
        onRequest?(...args: any[]): void
        onResponse?(...args: any[]): void
        onAbort?(...args: any[]): void
    };
}

export const mockServer = {
    mockHttpcalls: (httpCalls: HttpCall[]) => {
        cy.server();
        for (const hc of httpCalls) {
            const pro: Partial<Cypress.RouteOptions> = {
                method: (hc.method.toUpperCase() as Cypress.HttpMethod),
                url: hc.url,
                response: hc.response,
                delay: hc.delay,
                status: hc.status,
                headers: hc.headers,
            };

            if (hc.callbacks) {
                pro.onRequest = hc.callbacks.onRequest;
                pro.onResponse = hc.callbacks.onResponse;
                pro.onAbort = hc.callbacks.onAbort;
            }

            cy.route(pro);
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
