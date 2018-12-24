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
        max-rows="10"
        :header="dataTableHeaders"
        :data="dataTableData"
        placeholder="Search"
        @click="dataTableClick"
      >
        <!-- <template slot="date" slot-scope="{cell}">{{ new Date(cell.value).toDateString() }}</template> -->
        <template slot="age" slot-scope="{cell}">
          <div :class="$style.age">
            <div :style="{width: `${cell.value}%`}"/>
            <span>{{ cell.value }}</span>
          </div>
        </template>

        <template slot="commands" slot-scope="{row}">
          <vue-button @click="onDeleteRow(row)">Delete</vue-button>
        </template>
      </vue-data-table>
    </vue-grid-item>
  </div>
</template>

<script lang="ts">
import VueDataTable from "../shared/components/VueDataTable/VueDataTable.vue";
import { dataTableHeaderFixture } from "../shared/components/VueDataTable/DataTableFixtures";
import VueGrid from "../shared/components/VueGrid/VueGrid.vue";
import VueGridItem from "../shared/components/VueGridItem/VueGridItem.vue";
import VueGridRow from "../shared/components/VueGridRow/VueGridRow.vue";

export default {
  name: "HttpCall",
  metaInfo: {
    title: "http calls Management"
  },
  data(): any {
    return {
      dataTableData: [
        { id: 1, age: 11, name: "Roi" },
        { id: 2, age: 11, name: "Yohanna" },
        { id: 3, age: 10, name: "Uriyah" },
        { id: 4, age: 18, name: "Offer" },
        { id: 5, age: 12, name: "Yohanna" },
        { id: 6, age: 17, name: "Shay" },
        { id: 7, age: 15, name: "Roi" },
        { id: 8, age: 12, name: "Offer" },
        { id: 9, age: 13, name: "Yohanna" }
      ],
      dataTableHeaders: {
        id: {
          visible: true,
          title: "Id"
        },
        name: {
          title: "Name",
          slot: "name"
        },
        age: {
          title: "Age",
          slot: "age"
        },
        actions: {
          title: "Commands",
          slot: "commands"
        }
      }
    };
  },
  components: {
    VueGrid,
    VueGridItem,
    VueGridRow,
    VueDataTable
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
  text-shadow: 0 5px 10px rgba(0, 0, 0, 0.33);
  @include background-gradient($brand-dark-primary, $brand-accent, 152deg);
}

.collapseDemo {
  padding: $space-unit * 2;
  background: $panel-bg;
  box-shadow: $panel-shadow;
  color: #fff;
}

.age {
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
