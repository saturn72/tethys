<template>
  <div class="animated fadeIn">
  <b-row>
      <b-col sm="12">
        <b-card header="<i class='fa fa-align-justify'></i> Incoming Http Requests">
        <b-table :hover=true :striped=true :small=true :fixed=true responsive="sm" :items="items" :fields="fields" :current-page="currentPage" :per-page="perPage">
      <template slot="body" slot-scope="data">
        <b-badge :variant="getBadge(data.item.body)">{{data.item.body}}</b-badge>
      </template>
      <template slot="headers" slot-scope="data">
        <b-badge :variant="getBadge(data.item.headers)">{{data.item.headers}}</b-badge>
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
        { key: "resource" },
        { key: "query" },
        { key: "httpMethod" },
        { key: "body" },
        { key: "headers" }
      ],
      currentPage: 1,
      perPage: 10,
      totalRows: 0
    };
  },
  methods: {
    fetchEndpoints() {
      fetch("http://localhost:4880/tethys/api/log", {
        method: "GET",
        cache: "default"
      })
        .then(stream => stream.json())
        .then(
          data =>
            (this.items = data.map(d => {
              var req = d.request;
              return {
                resource: req.resource,
                query: req.query || "-",
                httpMethod: req.httpMethod,
                body: req.body,
                headers: req.headers
              };
            }))
        )
        .catch(err => console.error(err));
    },
    getBadge(body) {
      return body ? "content" : "no-content";
    },
    getRowCount(items) {
      return items.length;
    }
  },
  mounted() {
    this.fetchEndpoints();
  }
};
</script>
