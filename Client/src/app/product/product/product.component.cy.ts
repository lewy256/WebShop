import { ProductComponent } from './product.component'

describe('ProductComponent', () => {
  it('should mount', () => {
    cy.mount(ProductComponent)
  })
})