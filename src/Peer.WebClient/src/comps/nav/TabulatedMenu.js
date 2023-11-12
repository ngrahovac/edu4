import React, { useEffect } from 'react'
import { useState } from 'react';
import TabulatedMenuItem from './TabulatedMenuItem';

const TabulatedMenu = (props) => {
    const {
        items,
        onSelectionChange = () => {}
    } = props;

    const [selectedItem, setSelectedItem] = useState(undefined);

    useEffect(() => {
      if (selectedItem !== undefined) {
        onSelectionChange(selectedItem);
      }
    }, [selectedItem])
    

    return (
        <div className='w-full border-b-2 border-gray-200 flex flex-row gap-x-8'>
            {
                items.map(i => <>
                    <TabulatedMenuItem
                        onItemSelected={(item) => setSelectedItem(item)}
                        item={i}
                        selected={selectedItem === i}>
                    </TabulatedMenuItem>
                </>)
            }
        </div>
    )
}

export default TabulatedMenu