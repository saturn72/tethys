<template>
  <div class="animated fadeIn">
     <b-modal 
      id="error-modal"
      ref="modalError"
      title="Error"
      header-bg-variant="warning"
      v-model="error.show" 
      >
      <span v-html="error.body"></span>
    </b-modal>
    <b-row>
      <b-col md="6" class="my-1">
        <b-form-group horizontal label="Filter" class="mb-0">
          <b-input-group>
            <b-form-input v-model="filter" placeholder="Type to Search" />
            <b-input-group-append>
              <b-btn :disabled="!filter" @click="filter = ''">Clear</b-btn>
            </b-input-group-append>
          </b-input-group>
        </b-form-group>
      </b-col>
        <b-col sm="12">
          <b-card header="<i class='fa fa-align-justify'></i> Incoming Http Requests">          
            <label>Total HTTP Calls: <span class="badge badge-info">{{ items.length }}</span></label>
           <b-button-group size="sm" style="float:right">
              <b-button variant="css3" class="btn-brand" @click="reload"><i class="fa fa-refresh"></i><span>Reload</span></b-button>
              <b-button variant="secondary" v-bind:disabled="hasHiddens" class="btn-brand" @click="unhideAll"><i class="fa fa-eye"></i><span>Unhide All</span></b-button>
              <b-button variant="success" v-bind:disabled="hasLocked" class="btn-brand" @click="unlockAll"><i class="fa fa-unlock"></i><span>Unlock All</span></b-button>
              <b-button variant="warning" v-bind:disabled="!hasItems" class="btn-brand" @click="resetAllCalls"><i class="fa fa-trash"></i><span>Reset All Calls</span></b-button>
            </b-button-group>
            <b-table 
              :hover=true 
              :striped=true 
              :small=true 
              :fixed=true 
              responsive 
              :items="itemsToDisplay" 
              :fields="fields" 
              :current-page="currentPage" 
              :per-page="perPage" 
              :filter="filter"
              
              >
               <template slot="headers" slot-scope="row">
                 <ul>
                   <li v-for="(h, index) in headersToDisplay(row.item.headers)" :key="index">
                     { "{{ h.key }}" : {{ h.value }} }
                   </li>
                  </ul>
              </template>
              <template slot="actions" slot-scope="row">
                <b-button-group size="sm">
                   <router-link :to="{ name: 'Export', params: { id: row.item.id }}">                  
                    <b-button variant="primary" class="btn-brand"><i class="fa fa-download"></i><span>Export To Json</span></b-button>
                  </router-link>
                  <b-button variant="secondary" class="btn-brand" :disabled="isLocked(row.item.id)" @click="hide(row.item.id)"><i class="fa fa-eye-slash"></i><span>Hide</span></b-button>
                  <b-button variant="success" class="btn-brand" @click="toggleLock(row.item.id)"><i v-bind:class="[isLocked(row.item.id) ? 'fa fa-unlock': 'fa fa-lock']"></i><span>Lock</span></b-button>
                </b-button-group>
              </template>
            </b-table>
            <nav v-if="hasItems">
              <b-pagination :total-rows="itemsToDisplay.length" :per-page="perPage" v-model="currentPage" prev-text="Prev" next-text="Next" hide-goto-end-buttons/>
            </nav>
         </b-card>
      </b-col>
    </b-row>
  </div>
</template>
<script>
export default {
  name: "HttpList",
  data: () => {
    return {
      error: {
        show: false,
        body: ""
      },
      filter: null,
      items: [],
      fields: [
        { key: "id" },
        { key: "resource" },
        { key: "query" },
        { key: "httpMethod" },
        { key: "body" },
        { key: "headers" },
        { key: "actions" }
      ],
      currentPage: 1,
      perPage: 10
    };
  },
  computed: {
    hasItems: function() {
      return this.items.length > 0;
    },
    totalRows: function() {
      return this.itemsToDisplay.length;
    },
    hasHiddens: function() {
      return !this.items.some(itm => itm.hidden);
    },
    hasLocked: function() {
      return !this.items.some(itm => itm.locked);
    },
    itemsToDisplay: function() {
      return this.items.filter(itm => !itm.hidden || itm.locked);
    }
  },
  methods: {
    headersToDisplay: function(headers) {
      return Object.keys(headers).map(k => {
        return { key: k, value: headers[k] || "" };
      });
    },
    showModal(message) {
      this.error.body = "<p class='my-4'>" + message + "</p>";
      this.error.show = true;
    },
    onFiltered(filteredItems) {
      // Trigger pagination to update the number of buttons/pages due to filtering
      this.itemsToDisplay = filteredItems;
      this.currentPage = 1;
    },
    fetchHttpRequests() {
      var fetchUrl = baseUrl + "tethys/api/log";
      fetch(fetchUrl, {
        method: "GET",
        cache: "default"
      })
        .then(stream => stream.json())
        .then(
          data =>
            (this.items = data.reverse().map(d => {
              var req = d.request;
              const missingValue = "-";
              return {
                resource: req.resource || missingValue,
                query: req.query || missingValue,
                httpMethod: req.httpMethod || missingValue,
                body: req.body || missingValue,
                headers: req.headers || missingValue,
                id: d.id,
                hidden: false,
                locked: false
              };
            }))
        )
        .catch(err =>
          this.showModal(
            "failed to fetch from url: " + fetchUrl + "\nMore details: " + err
          )
        );
    },
    resetAllCalls() {
      var fetchUrl = baseUrl + "tethys/api/mock/reset";
      fetch(fetchUrl, {
        method: "DELETE",
        cache: "default"
      })
        .then(data => {
          this.items = [];
        })
        .catch(err =>
          this.showModal(
            "failed to fetch from url: " + fetchUrl + "\nMore details: " + err
          )
        );
    },
    reload() {
      this.fetchHttpRequests();
    },
    toggleLock(id) {
      var elemToLock = this.items.find(x => x.id === id);
      if (elemToLock) {
        elemToLock.locked = !elemToLock.locked;
      }
    },
    isLocked(id) {
      return this.items.find(x => x.id === id).locked;
    },
    unlockAll() {
      this.items.forEach(itm => (itm.locked = false));
    },
    hide(id) {
      var elemToHide = (this.items.find(x => x.id === id).hidden = true);
    },
    unhideAll() {
      this.items.forEach(itm => (itm.hidden = false));
    }
  },
  mounted() {
    this.fetchHttpRequests();
  }
};
const baseUrl = "http://localhost:4880/";
</script>
