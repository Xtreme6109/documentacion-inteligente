<template>
  <q-list>
    <q-expansion-item
      v-for="(group, category) in groupedLinks"
      :key="category"
      :label="category"
      expand-separator
    >
      <q-list>
        <q-item
          v-for="link in group"
          :key="link.title"
          clickable
          tag="router-link"
          :to="link.link"
        >
          <q-item-section v-if="link.icon" avatar>
            <q-icon :name="link.icon" />
          </q-item-section>

          <q-item-section>
            <q-item-label>{{ link.title }}</q-item-label>
          </q-item-section>
        </q-item>
      </q-list>
    </q-expansion-item>
  </q-list>
</template>

<script setup>
import { computed } from 'vue'

const props = defineProps({
  links: {
    type: Array,
    required: true
  }
})

// Agrupar por caption (Clientes, Facturación, etc.)
const groupedLinks = computed(() => {
  return props.links.reduce((groups, link) => {
    const category = link.caption || 'Otros'
    if (!groups[category]) groups[category] = []
    groups[category].push(link)
    return groups
  }, {})
})
</script>
