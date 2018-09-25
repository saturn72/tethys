<template>
<div class="app flex-row align-items-center">
  <b-modal 
    id="error-modal"
    ref="modalError"
    title="Error"
    header-bg-variant="warning"
    v-model="error.show" 
    >
    <span v-html="error.body"></span>
  </b-modal>   
    <div class="container">
      <b-row class="justify-content-center">
        <b-col>
          <b-card no-body class="mx-4">
            <b-card-body class="p-4">
              <b-form @submit="onSubmit" @reset="onReset">
                <h2>Export Http Request<b-badge pill variant="secondary" style="float:right">id: {{httpRequest.id}}</b-badge></h2>
                <b-input-group class="mb-3">
                  <b-button-group size="sm">
                    <b-button variant="css3" class="btn-brand" @click="onReset"><i class="fa fa-refresh"></i><span>Reset All</span></b-button>
                  </b-button-group>
                </b-input-group>
                <div id="request-details">
                  <p>
                    <b-btn v-b-toggle.request class="btn-brand" variant="primary" @click="isRequestCollapsed = !isRequestCollapsed">
                      <i v-bind:class="[isRequestCollapsed ? 'fa fa-plus-square': 'fa fa-minus-square']"></i><span> Request</span>
                      </b-btn>
                      <b-collapse v-bind:visible="!isRequestCollapsed" id="request" class="mt-2">

                      <b-input-group class="mb-3">
                        <b-input-group-prepend>
                          <b-input-group-text>Resource</b-input-group-text>
                        </b-input-group-prepend>
                        <b-form-input type="text" class="form-control" placeholder="resource" v-model="requestToDisplay.resource"/>
                        <b-button class="btn-brand" @click="resetRequestField('resource')"><i class="fa fa-refresh"></i></b-button>
                      </b-input-group>

                      <b-input-group class="mb-3" v-show="requestHasBody">
                        <b-input-group-prepend>
                          <b-input-group-text>Body</b-input-group-text>
                        </b-input-group-prepend>
                          <div id="jsoneditor" ref="requestEditor" style="width: 95%; height:400px;"> </div>
                        <b-button class="btn-brand" @click="resetRequestField('body')"><i class="fa fa-refresh"></i></b-button>
                      </b-input-group>
                    </b-collapse>
                  </p>
                </div>
                <div id="response-details">
                  <p>
                    <b-btn v-b-toggle.response class="btn-brand" variant="primary" @click="isResponseCollapsed = !isResponseCollapsed">
                      <i v-bind:class="[isRequestCollapsed ? 'fa fa-plus-square': 'fa fa-minus-square']"></i><span> Response</span>
                      </b-btn>
                      <b-collapse visible id="reponse" class="mt-2">
                        <b-input-group class="mb-3">
                          <b-input-group-prepend>
                            <b-input-group-text>Status Code</b-input-group-text>
                          </b-input-group-prepend>
                           <b-form-select v-model="responseToDisplay.statusCode" :options="httpStatusCodeOptions" required >
                          </b-form-select>
                          <b-button class="btn-brand" @click="resetResponseField('statusCode')"><i class="fa fa-refresh"></i></b-button>
                        </b-input-group>

                        <b-input-group class="mb-3">
                          <b-input-group-prepend>
                            <b-input-group-text>Body</b-input-group-text>
                          </b-input-group-prepend>
                            <div id="jsoneditor" ref="responseEditor" style="width: 95%; height:400px;"> </div>
                          <b-button class="btn-brand" @click="resetResponseField('body')"><i class="fa fa-refresh"></i></b-button>
                        </b-input-group>
                    </b-collapse>
                  </p>
                </div>
                <b-button type="submit" variant="primary" block>Export</b-button>
              </b-form>
           </b-card-body>
          </b-card>
        </b-col>
      </b-row>
    </div>
  </div>
</template>
<script>
import JsonEditor from "jsoneditor";
import { saveAs } from "file-saver/FileSaver";

export default {
  name: "HttpRequestExport",
  data: () => {
    return {
      error: {
        show: false,
        body: ""
      },
      isRequestCollapsed: true,
      isResponseCollapsed: false,
      serverData: {},
      httpRequest: {},
      dataToDisplay: {
        request: {},
        response: {}
      }
    };
  },
  created() {
    this.serverData.id = this.$route.params.id;
  },
  computed: {
    requestToDisplay: function() {
      return this.dataToDisplay.request;
    },
    responseToDisplay: function() {
      return this.dataToDisplay.response;
    },
    httpStatusCodeOptions: function() {
      return HttpStatusCodes.map(sc => {
        return {
          text: sc.code + " (" + sc.name + ")",
          value: sc.code
        };
      });
    },
    requestHasBody: function() {
      return this.dataToDisplay.request.body;
    },
    serverRequest: function() {
      return this.serverData.request;
    },
    serverResponse: function() {
      return this.serverData.response;
    }
  },
  methods: {
    showModal(message) {
      this.error.body = "<p class='my-4'>" + message + "</p>";
      this.error.show = true;
    },
    resetRequestField(field) {
      if (field === "body") {
        this.requestJsonEditor.setText(this.serverRequest.body);
      }
      this.dataToDisplay.request[field] = this.serverRequest[field];
    },
    resetResponseField(field) {
      if (field === "body") {
        this.responseJsonEditor.setText(this.serverResponse.body);
      }
      this.dataToDisplay.response[field] = this.serverResponse[field];
    },
    onSubmit() {
      this.dataToDisplay.response.body = this.responseJsonEditor.getText();
      var content = JSON.stringify(this.dataToDisplay);
      var file = new File([content], "req-res.json", {
        type: "text/plain;charset=utf-8"
      });
      saveAs(file);
    },
    onReset() {
      this.dataToDisplay.request = Object.assign({}, this.httpRequest);
      if (this.requestHasBody) {
        this.requestJsonEditor.setText(this.requestToDisplay.body);
      }
    },
    fetchHttpRequest() {
      var fetchUrl = baseUrl + "tethys/api/log/" + this.serverData.id;
      fetch(fetchUrl, {
        method: "GET",
        cache: "default"
      })
        .then(response => {
          if (!response || !response.ok) {
            throw "response retuened with status code: " + response.status;
          }
          return response.json();
        })
        .then(data => {
          this.serverData = data;
          this.httpRequest = data.request;
          this.onReset();
        })
        .catch(err =>
          this.showModal(
            "Failed to fetch from url: " +
              fetchUrl +
              "<br /><strong>More details:</strong> " +
              err
          )
        );
    },
    createJsonEditors() {
      var options = {};

      var requestContainer = this.$refs.requestEditor;
      this.requestJsonEditor = new JsonEditor(requestContainer, options);

      var responseContainer = this.$refs.responseEditor;
      this.responseJsonEditor = new JsonEditor(responseContainer, options);
    }
  },
  mounted() {
    this.fetchHttpRequest();
    this.createJsonEditors();
  }
};
const baseUrl = "http://localhost:4880/";

const HttpStatusCodes = [
  {
    name: "Continue",
    code: 100
  },
  {
    name: "Precondition Failed",
    code: 412
  },
  {
    name: "Request Entity Too Large",
    code: 413
  },
  {
    name: "Payload Too Large",
    code: 413
  },
  {
    name: "Request Uri Too Long",
    code: 414
  },
  {
    name: "Uri Too Long",
    code: 414
  },
  {
    name: "Unsupported Media Type",
    code: 415
  },
  {
    name: "Requested Range Not Satisfiable",
    code: 416
  },
  {
    name: "Range Not Satisfiable",
    code: 416
  },
  {
    name: "Expectation Failed",
    code: 417
  },
  {
    name: "Im A Teapot",
    code: 418
  },
  {
    name: "Authentication Timeout",
    code: 419
  },
  {
    name: "Misdirected Request",
    code: 421
  },
  {
    name: "Unprocessable Entity",
    code: 422
  },
  {
    name: "Locked",
    code: 423
  },
  {
    name: "Failed Dependency",
    code: 424
  },
  {
    name: "Upgrade Required",
    code: 426
  },
  {
    name: "Precondition Required",
    code: 428
  },
  {
    name: "TooMany Requests",
    code: 429
  },
  {
    name: "Request Header Fields TooLarge",
    code: 431
  },
  {
    name: "Unavailable For Legal Reasons",
    code: 451
  },
  {
    name: "Internal Server Error",
    code: 500
  },
  {
    name: "Not Implemented",
    code: 501
  },
  {
    name: "Bad Gateway",
    code: 502
  },
  {
    name: "Service Unavailable",
    code: 503
  },
  {
    name: "Gateway Timeout",
    code: 504
  },
  {
    name: "Http Version Notsupported",
    code: 505
  },
  {
    name: "Variant Also Negotiates",
    code: 506
  },
  {
    name: "Insufficient Storage",
    code: 507
  },
  {
    name: "Loop Detected",
    code: 508
  },
  {
    name: "Length Required",
    code: 411
  },
  {
    name: "Not Extended",
    code: 510
  },
  {
    name: "Gone",
    code: 410
  },
  {
    name: "Request Timeout",
    code: 408
  },
  {
    name: "Switching Protocols",
    code: 101
  },
  {
    name: "Processing",
    code: 102
  },
  {
    name: "OK",
    code: 200
  },
  {
    name: "Created",
    code: 201
  },
  {
    name: "Accepted",
    code: 202
  },
  {
    name: "Non Authoritative",
    code: 203
  },
  {
    name: "No Content",
    code: 204
  },
  {
    name: "Reset Content",
    code: 205
  },
  {
    name: "Partial Content",
    code: 206
  },
  {
    name: "Multi Status",
    code: 207
  },
  {
    name: "Already Reported",
    code: 208
  },
  {
    name: "I M Used",
    code: 226
  },
  {
    name: "Multiple Choices",
    code: 300
  },
  {
    name: "Moved Permanently",
    code: 301
  },
  {
    name: "Found",
    code: 302
  },
  {
    name: "See Other",
    code: 303
  },
  {
    name: "Not Modified",
    code: 304
  },
  {
    name: "Use Proxy",
    code: 305
  },
  {
    name: "Switch Proxy",
    code: 306
  },
  {
    name: "Temporary Redirect",
    code: 307
  },
  {
    name: "Permanent Redirect",
    code: 308
  },
  {
    name: "Bad Request",
    code: 400
  },
  {
    name: "Unauthorized",
    code: 401
  },
  {
    name: "Payment Required",
    code: 402
  },
  {
    name: "For bidden",
    code: 403
  },
  {
    name: "Not Found",
    code: 404
  },
  {
    name: "Method Not Allowed",
    code: 405
  },
  {
    name: "Not Acceptable",
    code: 406
  },
  {
    name: "Proxy Authentication Required",
    code: 407
  },
  {
    name: "Conflict",
    code: 409
  },
  {
    name: "Network Authentication Required",
    code: 511
  }
];
</script>
