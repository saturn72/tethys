<template>
  <div class="animated fadeIn">
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
              <b-button variant="warning" class="btn-brand" @click="resetAllCalls"><i class="fa fa-trash"></i><span>Reset All Calls</span></b-button>
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
              <template slot="commands" slot-scope="row">
                <b-button-group size="sm">
                  <b-button variant="primary" @click="exportRequest(row.item.id)">Export To Json</b-button>
                  <b-button variant="secondary" class="btn-brand" :disabled="isLocked(row.item.id)" @click="hide(row.item.id)"><i class="fa fa-eye-slash"></i><span>Hide</span></b-button>
                  <b-button variant="success" class="btn-brand" @click="toggleLock(row.item.id)"><i v-bind:class="[isLocked(row.item.id) ? 'fa fa-unlock': 'fa fa-lock']"></i><span>Lock</span></b-button>
                </b-button-group>
              </template>
            </b-table>
            <nav>
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
      filter: null,
      items: [],
      fields: [
        { key: "id" },
        { key: "resource" },
        { key: "query" },
        { key: "httpMethod" },
        { key: "body" },
        { key: "headers" },
        { key: "commands" }
      ],
      currentPage: 1,
      perPage: 10
    };
  },
  computed: {
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
    onFiltered(filteredItems) {
      // Trigger pagination to update the number of buttons/pages due to filtering
      this.itemsToDisplay = filteredItems;
      this.currentPage = 1;
    },
    exportRequest(id) {
      console.log(id + " should be exported");
    },
    fetchEndpoints() {
      fetch(baseUrl + "tethys/api/log", {
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
        .catch(err => console.error(err));
    },
    resetAllCalls() {
      fetch(baseUrl + "tethys/api/mock/reset", {
        method: "DELETE",
        cache: "default"
      })
        .then(data => {
          this.items = [];
        })
        .catch(err => console.error(err));
    },
    reload() {
      this.fetchEndpoints();
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
    // onFiltered(filteredItems) {
    //   console.log(filteredItems);
    //   var lockedItems = this.items.filter(itm => itm.locked);

    //   filteredItems = lockedItems.concat(filteredItems);

    //   this.currentPage = 1;
    // }
  },
  mounted() {
    this.fetchEndpoints();
  }
};
const baseUrl = "http://localhost:4880/";
</script>
