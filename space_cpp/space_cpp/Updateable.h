#ifndef UPDATEABLE_H
#define UPDATEABLE_H

#include <Gem\Org\DList.h>

class Updateable : public Gem::dListNode<Updateable> {
protected:
	Updateable();
	~Updateable();
public: 
	virtual void update() = 0;
};


#endif //UPDATEABLE_H