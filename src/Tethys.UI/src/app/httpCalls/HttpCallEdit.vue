<template>
  <h1>this is edit</h1>
</template>

<script lang="ts">
export default {
  name: "HttpCallEdit",
  metaInfo: {
    title: "http call edit"
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
