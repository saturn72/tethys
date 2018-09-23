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
              <b-form>
                <h2>Export Http Request<b-badge pill variant="secondary" style="float:right">id: {{httpRequest.id}}</b-badge></h2>
                <b-input-group class="mb-3">
                  <b-button-group size="sm">
                    <b-button variant="css3" class="btn-brand" @click="resetAll"><i class="fa fa-refresh"></i><span>Reset All</span></b-button>
                  </b-button-group>
                </b-input-group>
                <b-input-group class="mb-3">
                  <b-input-group-prepend>
                    <b-input-group-text>Resource</b-input-group-text>
                  </b-input-group-prepend>
                  <b-form-input type="text" class="form-control" placeholder="resource" v-model="dataToDisplay.resource"/>
                  <b-button class="btn-brand" @click="reset('resource')"><i class="fa fa-refresh"></i></b-button>
                </b-input-group>

                <b-input-group class="mb-3">
                  <b-input-group-prepend>
                    <b-input-group-text>Body</b-input-group-text>
                  </b-input-group-prepend>
                    <div id="jsoneditor" ref="editor" style="width: 95%; height:400px;"> </div>
                  <b-button class="btn-brand" @click="reset('resource')"><i class="fa fa-refresh"></i></b-button>
                </b-input-group>
                <b-button variant="primary" block>Export</b-button>
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

export default {
  name: "HttpRequestExport",
  data: () => {
    return {
      error: {
        show: false,
        body: ""
      },
      httpRequest: {},
      dataToDisplay: {}
    };
  },
  created() {
    this.httpRequest.id = this.$route.params.id;
  },
  methods: {
    showModal(message) {
      this.error.body = "<p class='my-4'>" + message + "</p>";
      this.error.show = true;
    },
    reset(propName) {
      if (!propName) {
        return reselAll();
      }
      this.dataToDisplay[propName] = this.httpRequest[propName];
    },
    resetAll() {
      this.dataToDisplay = Object.assign({}, this.httpRequest);
      console.log(this.dataToDisplay.body);
      this.jsonEditor.setText(this.dataToDisplay.body);
    },
    fetchHttpRequest() {
      var fetchUrl = baseUrl + "tethys/api/log/" + this.httpRequest.id;
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
          this.httpRequest = data.request;
          this.resetAll();
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
    createJsonEditor() {
      var container = this.$refs.editor;
      var options = {};
      this.jsonEditor = new JsonEditor(container, options);
    }
  },
  mounted() {
    this.fetchHttpRequest();
    this.createJsonEditor();
  }
};
const baseUrl = "http://localhost:4880/";
</script>
