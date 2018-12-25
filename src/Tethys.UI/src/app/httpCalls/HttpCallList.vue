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
        {
          id: 1,
          httpMethod: "GET|OPTIONS",
          usage: 52,
          name: "Laurel"
        },
        {
          id: 2,
          httpMethod: "GET|OPTIONS",
          usage: 50,
          name: "Ollie"
        },
        {
          id: 3,
          httpMethod: "GET",
          usage: 52,
          name: "Tersina"
        },
        {
          id: 4,
          httpMethod: "POST",
          usage: 1,
          name: "Anatole"
        },
        {
          id: 5,
          httpMethod: "DELETE",
          usage: 97,
          name: "Lacee"
        },
        {
          id: 6,
          httpMethod: "PUT",
          usage: 26,
          name: "Karita"
        },
        {
          id: 7,
          httpMethod: "GET|OPTIONS",
          usage: 22,
          name: "Marabel"
        },
        {
          id: 8,
          httpMethod: "GET|OPTIONS",
          usage: 2,
          name: "Martie"
        },
        {
          id: 9,
          httpMethod: "GET",
          usage: 53,
          name: "Davine"
        },
        {
          id: 10,
          httpMethod: "POST",
          usage: 74,
          name: "Bab"
        },
        {
          id: 11,
          httpMethod: "GET",
          usage: 29,
          name: "Moshe"
        },
        {
          id: 12,
          httpMethod: "POST",
          usage: 69,
          name: "Herold"
        },
        {
          id: 13,
          httpMethod: "GET|OPTIONS",
          usage: 74,
          name: "Kordula"
        },
        {
          id: 14,
          httpMethod: "POST",
          usage: 54,
          name: "Austina"
        },
        {
          id: 15,
          httpMethod: "GET|OPTIONS",
          usage: 64,
          name: "Dewain"
        },
        {
          id: 16,
          httpMethod: "GET",
          usage: 31,
          name: "Lyndel"
        },
        {
          id: 17,
          httpMethod: "GET|OPTIONS",
          usage: 33,
          name: "Letizia"
        },
        {
          id: 18,
          httpMethod: "GET",
          usage: 2,
          name: "Fulton"
        },
        {
          id: 19,
          httpMethod: "GET",
          usage: 87,
          name: "Penelope"
        },
        {
          id: 20,
          httpMethod: "GET|OPTIONS",
          usage: 26,
          name: "Claude"
        },
        {
          id: 21,
          httpMethod: "OPTIONS",
          usage: 22,
          name: "Umberto"
        },
        {
          id: 22,
          httpMethod: "POST",
          usage: 47,
          name: "Gradeigh"
        },
        {
          id: 23,
          httpMethod: "GET|OPTIONS",
          usage: 12,
          name: "Harbert"
        },
        {
          id: 24,
          httpMethod: "GET|OPTIONS",
          usage: 84,
          name: "Klemens"
        },
        {
          id: 25,
          httpMethod: "GET",
          usage: 14,
          name: "Teresina"
        },
        {
          id: 26,
          httpMethod: "DELETE",
          usage: 35,
          name: "Calida"
        },
        {
          id: 27,
          httpMethod: "OPTIONS",
          usage: 34,
          name: "Val"
        },
        {
          id: 28,
          httpMethod: "PUT",
          usage: 26,
          name: "Cybill"
        },
        {
          id: 29,
          httpMethod: "PUT",
          usage: 92,
          name: "Findlay"
        },
        {
          id: 30,
          httpMethod: "GET|OPTIONS",
          usage: 61,
          name: "Regan"
        },
        {
          id: 31,
          httpMethod: "GET|OPTIONS",
          usage: 76,
          name: "Lacey"
        },
        {
          id: 32,
          httpMethod: "DELETE",
          usage: 38,
          name: "Elysia"
        },
        {
          id: 33,
          httpMethod: "DELETE",
          usage: 50,
          name: "Celina"
        },
        {
          id: 34,
          httpMethod: "PUT",
          usage: 94,
          name: "Noni"
        },
        {
          id: 35,
          httpMethod: "GET|OPTIONS",
          usage: 42,
          name: "Stanly"
        },
        {
          id: 36,
          httpMethod: "PUT",
          usage: 47,
          name: "Darda"
        },
        {
          id: 37,
          httpMethod: "GET|OPTIONS",
          usage: 74,
          name: "Eugine"
        },
        {
          id: 38,
          httpMethod: "GET",
          usage: 37,
          name: "Keriann"
        },
        {
          id: 39,
          httpMethod: "OPTIONS",
          usage: 23,
          name: "Ester"
        },
        {
          id: 40,
          httpMethod: "GET|OPTIONS",
          usage: 56,
          name: "Fay"
        },
        {
          id: 41,
          httpMethod: "GET|OPTIONS",
          usage: 94,
          name: "Gaye"
        },
        {
          id: 42,
          httpMethod: "GET|OPTIONS",
          usage: 23,
          name: "Durand"
        },
        {
          id: 43,
          httpMethod: "GET|OPTIONS",
          usage: 44,
          name: "Correna"
        }
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
