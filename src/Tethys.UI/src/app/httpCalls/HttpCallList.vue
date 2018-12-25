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

    <vue-grid-item class="vueGridItem">
      <h2>Http Calls</h2>
      <vue-data-table
        :max-rows="10"
        :header="dataTableHeaders"
        :data="dataTableData"
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
          <vue-button @click="onHttpCallClicked(row)">
            <span>
              <font-awesome-icon icon="info-circle"/>
            </span>&nbsp;Details
          </vue-button>
        </template>
      </vue-data-table>
    </vue-grid-item>
  </div>
</template>

<script lang="ts">
import VueDataTable from "../shared/components/VueDataTable/VueDataTable.vue";
import VueGrid from "../shared/components/VueGrid/VueGrid.vue";
import VueGridItem from "../shared/components/VueGridItem/VueGridItem.vue";
import VueGridRow from "../shared/components/VueGridRow/VueGridRow.vue";
import VueButton from "../shared/components/VueButton/VueButton.vue";

import { library } from "@fortawesome/fontawesome-svg-core";
import { faInfoCircle } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/vue-fontawesome";

library.add(faInfoCircle);

export default {
  name: "HttpCall",
  metaInfo: {
    title: "http calls Management"
  },
  data(): any {
    return {
      dataTableData: [
        { id: 1, httpMethod: "GET", usage: 30, name: "Roi" },
        { id: 2, httpMethod: "GET", usage: 20, name: "Yohanna" },
        { id: 3, httpMethod: "GET|OPTIONS", usage: 100, name: "Uriyah" },
        { id: 4, httpMethod: "POST", usage: 15, name: "Offer" },
        { id: 5, httpMethod: "DELETE", usage: 73, name: "Yohanna" },
        { id: 7, httpMethod: "GET", usage: 100, name: "Roi" },
        { id: 8, httpMethod: "DELETE", usage: 26, name: "Offer" },
        { id: 9, httpMethod: "GET", usage: 98, name: "Yohanna" }
      ],
      dataTableHeaders: {
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
  methods: {
    onHttpCallClicked(httpCall: any) {
      alert("raw was clicked: " + JSON.stringify(httpCall));
    }
  },
  components: {
    VueButton,
    VueGrid,
    VueGridItem,
    VueGridRow,
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

.collapseDemo {
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
