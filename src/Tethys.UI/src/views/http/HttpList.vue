<template>
  <div class="animated fadeIn">
    <b-row>
        <b-col sm="12">
          <b-card header="<i class='fa fa-align-justify'></i> Incoming Http Requests">          
            <label>Total HTTP Calls: <span class="badge badge-info">{{ items.length }}</span></label>
           <b-button-group size="sm" style="float:right">
              <b-button variant="css3" class="btn-brand" @click="refresh"><i class="fa fa-refresh"></i><span>Refresh</span></b-button>
              <b-button variant="secondary" :disabled="hasHiddens" class="btn-brand" @click="unhideAll"><i class="fa fa-eye"></i><span>Unhide All</span></b-button>
              <b-button variant="warning" class="btn-brand" @click="resetAllCalls"><i class="fa fa-trash"></i><span>Reset All Calls</span></b-button>
            </b-button-group>
            <b-table :hover=true :striped=true :small=true :fixed=true responsive="sm" :items="itemsToDisplay" :fields="fields" :current-page="currentPage" :per-page="perPage">
              <template slot="commands" slot-scope="row">
                <b-button-group size="sm">
                  <b-button variant="primary" @click="exportRequest(row.item.id)">Export To Json</b-button>
                  <b-button variant="secondary" class="btn-brand" @click="hide(row.item.id)"><i class="fa fa-eye-slash"></i><span>Hide</span></b-button>
                </b-button-group>
              </template>
            </b-table>
            <nav>
              <b-pagination :total-rows="getRowCount(items)" :per-page="perPage" v-model="currentPage" prev-text="Prev" next-text="Next" hide-goto-end-buttons/>
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
      perPage: 10,
      totalRows: 0
    };
  },
  computed: {
    hasHiddens: function() {
      return this.items.some(itm => !itm.hidden);
    },
    itemsToDisplay: function() {
      return this.items.filter(itm => !itm.hidden);
    }
  },
  methods: {
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
                hidden: false
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
    refresh() {
      this.fetchEndpoints();
    },
    hide(id) {
      var elemToHide = this.items.find(x => x.id === id);
      if (elemToHide) {
        elemToHide.hidden = true;
      }
    },
    unhideAll() {
      this.items.forEach(itm => (itm.hidden = false));
    },
    getRowCount(items) {
      return items.length;
    }
  },
  mounted() {
    this.fetchEndpoints();
  }
};
const baseUrl = "http://localhost:4880/";
</script>
