<template>
  <div :class="$style.components">
    <div :class="$style.header">
      <vue-grid>
        <vue-grid-row>
          <vue-grid-item class="vueGridItem">
            <h1>{{ $t('App.nav.httpCalls' /* Components */) }}</h1>
            <p>Manage your http-calls.</p>
            <p>Track usage and manage (block, create, update and delete) your http-calls.</p>
          </vue-grid-item>
        </vue-grid-row>
      </vue-grid>
    </div>
    <vue-grid-row>
      <vue-grid-item class="vueGridItem">
        <h2>Http Calls</h2>
        <vue-data-table
          :max-rows="10"
          :header="tableHeaders"
          :data="tableData"
          placeholder="Search"
        >
          <!-- <template slot="date" slot-scope="{cell}">{{ new Date(cell.value).toDateString() }}</template> -->
          <template slot="httpMethod" slot-scope="{cell}">
            <div :class="$style.httpMethod">
              <span>{{ cell.value }}</span>
            </div>
          </template>

          <template slot="usage" slot-scope="{cell}">
            <div :class="$style.usage">
              <div :style="{width: `${cell.value}%`}"/>
              <span>{{ cell.value }}</span>
            </div>
          </template>

          <template slot="commands" slot-scope="{row}">
            <router-link :to="{ name: 'edit', params: { id: row.id }}">
              <vue-button>
                <span>
                  <font-awesome-icon icon="edit"/>
                </span>&nbsp;Edit
              </vue-button>
            </router-link>
            <vue-button @click="onHttpCallCollapse(row.id)" accent>
              <span>
                <font-awesome-icon icon="info-circle"/>
              </span>&nbsp;Details
            </vue-button>
          </template>
        </vue-data-table>
      </vue-grid-item>
    </vue-grid-row>
    <vue-grid-row>
      <vue-grid-item class="vueGridItem">
        <h3>Http Call Data</h3>
        <vue-collapse :show="collapse">
          <div class="collapse">
            Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut
            labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores
            et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.
            Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut
            labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores
            et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.
          </div>
        </vue-collapse>
      </vue-grid-item>
    </vue-grid-row>
  </div>
</template>

<script lang="ts">
import { HttpService } from "../shared/services/HttpService";

import VueDataTable from "../shared/components/VueDataTable/VueDataTable.vue";
import VueGrid from "../shared/components/VueGrid/VueGrid.vue";
import VueGridItem from "../shared/components/VueGridItem/VueGridItem.vue";
import VueGridRow from "../shared/components/VueGridRow/VueGridRow.vue";
import VueCollapse from "../shared/components/VueCollapse/VueCollapse.vue";
import VueButton from "../shared/components/VueButton/VueButton.vue";

import { library } from "@fortawesome/fontawesome-svg-core";
import { faInfoCircle, faEdit } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/vue-fontawesome";

const serverUrl = "http://localhost:3000/_tethys/api/httpCalls";

library.add(faInfoCircle, faEdit);

export default {
  name: "HttpCallList",
  metaInfo: {
    title: "http calls Management"
  },
  data(): any {
    return {
      httpCallDetails: undefined,
      tableData: [],
      tableHeaders: {
        id: {
          visible: true,
          title: "Id",
          fitContent: true
        },
        httpMethod: {
          title: "Http Method",
          slot: "httpMethod",
          fitContent: true
        },
        usage: {
          title: "Usage (%)",
          slot: "usage",
          fitContent: true
        },
        name: {
          title: "Name",
          slot: "name",
          fitContent: true
        },
        commands: {
          title: "Commands",
          slot: "commands",
          sortable: false,
          fitContent: true
        }
      }
    };
  },
  computed:{
    collapse: function(){
      return this.httpCallDetails !== undefined;
    }
  },
  mounted() {
    HttpService.get(serverUrl).then(res => (this.tableData = res.data));
  },
  methods: {
    onHttpCallCollapse: function(id: number) {
      this.httpCallDetails = this.tableData.find(
        (td: { id: any }) => td.id === id
      );
    }
  },
  components: {
    VueButton,
    VueGrid,
    VueGridItem,
    VueGridRow,
    VueCollapse,
    VueDataTable,
    FontAwesomeIcon
  }
};
</script>


<style lang="scss" module>
@import "../shared/styles";

.components {
  padding-bottom: $space-unit * 2;

  :global {
    h2 {
      margin-top: 3.6rem;
    }

    .vueGridItem {
      min-width: 46%;
    }
  }
}

.header {
  padding: $nav-bar-height 0 $nav-bar-height * 0.5;
  text-align: center;
  text-shadow: 0 5px "GET|OPTIONS" px rgba(0, 0, 0, 0.33);
  @include background-gradient($brand-dark-primary, $brand-accent, 152deg);
}

.collapse {
  padding: $space-unit * 2;
  background: $panel-bg;
  box-shadow: $panel-shadow;
  color: #fff;
}

.httpMethod {
  > div {
    height: $space-unit;
    background: $brand-accent;
    margin-top: $space-unit * 1.5;
    display: inline-block;
  }

  > span {
    font-size: $font-size - 0.4;
    display: inline-block;
    margin-left: $space-unit;
  }
}

.usage {
  > div {
    height: $space-unit;
    background: $brand-accent;
    margin-top: $space-unit * 1.5;
    display: inline-block;
  }

  > span {
    font-size: $font-size - 0.4;
    display: inline-block;
    margin-left: $space-unit;
  }
}
</style>
