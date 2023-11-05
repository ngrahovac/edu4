import React from 'react'

const SectionTitleWrapper = (props) => {
    const {
        children
    } = props;

  return (
    <div className='mb-8'>
        {children}
    </div>
  )
}

export default SectionTitleWrapper